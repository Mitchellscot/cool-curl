using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoolCurl.Models;
using CoolCurl.Services;

namespace CoolCurl.Infrastructure;

public class GeminiService : IAiClient
{
    private readonly ConfigurationService _configService;
    private readonly HttpClient _httpClient;
    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent";

    private const string SystemPrompt = @"You are a curl command generator assistant. Your job is to generate ONLY a valid curl command based on the user's request and their configuration.

CRITICAL RULES - USER REQUEST ALWAYS WINS:
1. Generate ONLY the curl command - no explanations, no markdown, no extra text
2. **ALWAYS PRIORITIZE the user's explicit request over ANY configuration defaults**
3. If the user specifies an HTTP method (GET, POST, PUT, DELETE, PATCH, etc.), use EXACTLY that method - IGNORE the default method from configuration
4. If the user specifies a path or endpoint, use EXACTLY that path - do NOT use paths from configuration
5. If the user specifies headers, query parameters, or data, use EXACTLY what they specify - do NOT add defaults unless the user asks
6. Use the base URL from configuration ONLY if the user doesn't provide a full URL
7. Include authentication headers ONLY if already in configuration (use placeholder ""REPLACEME"" for tokens/passwords)
8. Include default headers from configuration ONLY if they don't conflict with user's request
9. For POST/PUT/PATCH requests, include -d flag with data ONLY if the user explicitly mentions sending data
10. Add flags like -L, -i, --show-error based on configuration settings
11. The output must be a single line, executable curl command
12. Do NOT include any explanations, markdown code blocks, or additional text

EXAMPLES:
User request: GET /api/users -> curl -X GET ""https://api.example.com/api/users""
User request: POST to /users with name John -> curl -X POST -H ""Content-Type: application/json"" ""https://api.example.com/users"" -d '{""name"":""John""}'
User request: delete /products/123 -> curl -X DELETE ""https://api.example.com/products/123""

For Basic Auth, use: -u REPLACEME or -H ""Authorization: Basic REPLACEME""

REMEMBER: User's explicit request ALWAYS overrides configuration defaults!";
    public GeminiService(ConfigurationService configService)
    {
        _configService = configService;
        _httpClient = new HttpClient();
    }

    public async Task<string?> GenerateCurlCommandAsync(string userPrompt)
    {
        var settings = _configService.GetSettings();

        if (string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured. Use -gk to set your API key.");
        }

        var context = BuildConfigurationContext(settings);
        var fullPrompt = $"{SystemPrompt}\n\nUser Configuration:\n{context}\n\nUser Request: {userPrompt}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = fullPrompt }
                    }
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, GeminiApiUrl)
        {
            Content = content
        };
        request.Headers.Add("x-goog-api-key", settings.GeminiApiKey);

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
                    Console.WriteLine("Gemini API rate limit exceeded (15 requests/min or 1,500/day on free tier). Please wait a moment and try again.");
                }
                else if (statusCode == 403)
                {
                    Console.WriteLine("Gemini API permission denied. Check your API key with -gk or ensure the Generative Language API is enabled.");
                }
                else if (statusCode == 401)
                {
                    Console.WriteLine("Gemini API authentication failed. Check your API key with -gk");
                }
                else
                {
                    Console.WriteLine($"Gemini API error ({statusCode}): {errorBody}");
                }
                Console.ResetColor();
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Length > 0)
            {
                var candidate = geminiResponse.Candidates[0];
                if (candidate?.Content?.Parts != null && candidate.Content.Parts.Length > 0)
                {
                    var text = candidate.Content.Parts[0]?.Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var curlCommand = text.Trim();

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
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to communicate with Gemini API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse Gemini API response: {ex.Message}", ex);
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

        if (string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured. Use -gk to set your API key.");
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
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = debugPrompt }
                    }
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, GeminiApiUrl)
        {
            Content = content
        };
        request.Headers.Add("x-goog-api-key", settings.GeminiApiKey);

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
                    Console.WriteLine("Gemini API rate limit exceeded (15 requests/min or 1,500/day on free tier). Please wait a moment and try again.");
                }
                else if (statusCode == 403)
                {
                    Console.WriteLine("Gemini API permission denied. Check your API key with -gk or ensure the Generative Language API is enabled.");
                }
                else if (statusCode == 401)
                {
                    Console.WriteLine("Gemini API authentication failed. Check your API key with -gk");
                }
                else
                {
                    Console.WriteLine($"Gemini API error ({statusCode}): {errorBody}");
                }
                Console.ResetColor();
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Length > 0)
            {
                var candidate = geminiResponse.Candidates[0];
                if (candidate?.Content?.Parts != null && candidate.Content.Parts.Length > 0)
                {
                    var text = candidate.Content.Parts[0]?.Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return text.Trim();
                    }
                }
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to communicate with Gemini API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse Gemini API response: {ex.Message}", ex);
        }
    }
}
