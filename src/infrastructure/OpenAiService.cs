using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoolCurl.Models;
using CoolCurl.Services;

namespace CoolCurl.Infrastructure;

public class OpenAiService : IAiClient
{
    private readonly ConfigurationService _configService;
    private readonly HttpClient _httpClient;
    private const string OpenAiApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string Model = "gpt-4o-mini";

    private const string SystemPrompt = @"You are a curl command generator assistant. Your job is to generate ONLY a valid curl command based on the user's request and their configuration.

Rules:
1. Generate ONLY the curl command - no explanations, no markdown, no extra text
2. PRIORITIZE the user's explicit request over configuration defaults
3. If the user specifies an HTTP method (GET, POST, PUT, DELETE, etc.), use THAT method, not the default from configuration
4. If the user specifies a path, use ONLY that path - do not add query parameters unless explicitly requested
5. Use the base URL from the configuration
6. Include authentication headers if specified in the configuration - use the placeholder ""REPLACEME"" for tokens/passwords
7. Include default headers from the configuration
8. Add appropriate flags like -L (follow redirects), -i (show headers), --show-error, etc. based on configuration
9. For POST/PUT/PATCH requests, include appropriate -d flag with sample JSON data if needed
10. The output must be a single line, executable curl command
11. Do NOT include any explanations or additional text
12. IMPORTANT: For any authentication tokens, API keys, or passwords, always use the exact string ""REPLACEME"" as a placeholder

Example output format:
curl -X POST -H ""Authorization: Bearer REPLACEME"" -H ""Content-Type: application/json"" ""https://api.example.com/users"" -d '{""name"":""John""}'

For Basic Auth, use: -u REPLACEME or -H ""Authorization: Basic REPLACEME""";

    public OpenAiService(ConfigurationService configService)
    {
        _configService = configService;
        _httpClient = new HttpClient();
    }

    public async Task<string?> GenerateCurlCommandAsync(string userPrompt)
    {
        var settings = _configService.GetSettings();

        if (string.IsNullOrWhiteSpace(settings.OpenAiApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Use -ok to set your API key.");
        }

        var context = BuildConfigurationContext(settings);
        var fullPrompt = $"{SystemPrompt}\n\nUser Configuration:\n{context}\n\nUser Request: {userPrompt}";

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = $"User Configuration:\n{context}\n\nUser Request: {userPrompt}" }
            },
            temperature = 0.7
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, OpenAiApiUrl)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Bearer {settings.OpenAiApiKey}");

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode;

                Console.ForegroundColor = ConsoleColor.Red;
                if (statusCode == 429)
                {
                    Console.WriteLine("OpenAI API rate limit exceeded. Please wait a moment and try again, or check your usage at https://platform.openai.com/usage");
                }
                else if (statusCode == 401)
                {
                    Console.WriteLine("OpenAI API authentication failed. Check your API key with -ok");
                }
                else
                {
                    Console.WriteLine($"OpenAI API error ({statusCode}): {errorBody}");
                }
                Console.ResetColor();
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseBody);

            if (openAiResponse?.Choices != null && openAiResponse.Choices.Length > 0)
            {
                var choice = openAiResponse.Choices[0];
                if (choice?.Message?.Content != null)
                {
                    var curlCommand = choice.Message.Content.Trim();

                    if (settings.AuthType == AuthType.BearerToken || settings.AuthType == AuthType.JwtBearer)
                    {
                        if (!string.IsNullOrWhiteSpace(settings.AuthToken))
                        {
                            curlCommand = curlCommand.Replace("REPLACEME", settings.AuthToken);
                        }
                    }
                    else if (settings.AuthType == AuthType.BasicAuth)
                    {
                        if (!string.IsNullOrWhiteSpace(settings.BasicAuthUsername))
                        {
                            var credentials = settings.BasicAuthUsername;
                            if (!string.IsNullOrWhiteSpace(settings.BasicAuthPassword))
                            {
                                credentials += ":" + settings.BasicAuthPassword;
                            }
                            curlCommand = curlCommand.Replace("REPLACEME", credentials);
                        }
                    }

                    return curlCommand;
                }
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to communicate with OpenAI API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse OpenAI API response: {ex.Message}", ex);
        }
    }

    private string BuildConfigurationContext(Settings settings)
    {
        // Only mask headers that might contain sensitive data
        var sensitiveHeaderNames = new[] { "Authorization", "X-API-Key", "X-Auth-Token", "Cookie", "X-CSRF-Token" };
        var maskedHeaders = settings.DefaultHeaders?.ToDictionary(
            kvp => kvp.Key,
            kvp => sensitiveHeaderNames.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase) ? "REPLACEME" : kvp.Value
        );

        var context = new
        {
            baseUrl = settings.BaseUrl ?? "NOT_SET",
            defaultMethod = settings.DefaultMethod ?? "GET",
            authType = settings.AuthType.ToString(),
            hasAuthToken = !string.IsNullOrWhiteSpace(settings.AuthToken),
            hasBasicAuthUsername = !string.IsNullOrWhiteSpace(settings.BasicAuthUsername),
            hasBasicAuthPassword = !string.IsNullOrWhiteSpace(settings.BasicAuthPassword),
            followRedirects = settings.FollowRedirects,
            showProgress = settings.ShowProgress,
            showError = settings.ShowError,
            showHeaders = settings.ShowHeaders,
            maxTimeSeconds = settings.MaxTimeSeconds,
            defaultHeaders = maskedHeaders,
            queryParameters = settings.QueryParameters,
            recentPaths = settings.RecentPaths
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(context, options);
    }

    public async Task<string?> DebugHttpErrorAsync(string errorMessage, string? requestDetails = null)
    {
        var settings = _configService.GetSettings();

        if (string.IsNullOrWhiteSpace(settings.OpenAiApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Use -ok to set your API key.");
        }

        var debugPrompt = $@"You are an HTTP debugging assistant. Analyze the following HTTP error and provide brief, actionable troubleshooting suggestions.

Error: {errorMessage}

{(string.IsNullOrWhiteSpace(requestDetails) ? "" : $"Request Details:\n{requestDetails}\n")}

Provide a concise response (3-5 bullet points max):
1. Most likely cause
2. 1-2 specific steps to fix the issue
3. Any relevant curl flags or configuration changes

Keep it brief and actionable. Avoid lengthy explanations.";

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = "You are an HTTP debugging assistant. Provide brief, actionable troubleshooting suggestions." },
                new { role = "user", content = debugPrompt }
            },
            temperature = 0.7
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, OpenAiApiUrl)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Bearer {settings.OpenAiApiKey}");

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode;

                Console.ForegroundColor = ConsoleColor.Red;
                if (statusCode == 429)
                {
                    Console.WriteLine("OpenAI API rate limit exceeded. Please wait a moment and try again, or check your usage at https://platform.openai.com/usage");
                }
                else if (statusCode == 401)
                {
                    Console.WriteLine("OpenAI API authentication failed. Check your API key with -ok");
                }
                else
                {
                    Console.WriteLine($"OpenAI API error ({statusCode}): {errorBody}");
                }
                Console.ResetColor();
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseBody);

            if (openAiResponse?.Choices != null && openAiResponse.Choices.Length > 0)
            {
                var choice = openAiResponse.Choices[0];
                if (choice?.Message?.Content != null)
                {
                    return choice.Message.Content.Trim();
                }
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to communicate with OpenAI API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse OpenAI API response: {ex.Message}", ex);
        }
    }
}
