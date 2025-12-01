using CoolCurl.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoolCurl.Services;

public class HttpClientService
{
    private readonly ConfigurationService _configService;
    private readonly HttpClient _httpClient;

    public HttpClientService(ConfigurationService configService)
    {
        _configService = configService;
        _httpClient = new HttpClient();
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        var settings = _configService.GetSettings();

        // Set timeout if configured
        if (settings.MaxTimeSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(settings.MaxTimeSeconds.Value);
        }

        // Configure to follow redirects
        // Note: HttpClient follows redirects by default, but we can control it via HttpClientHandler
    }

    public async Task<HttpResponseMessage> ExecuteRequestAsync(string? pathOverride = null)
    {
        var settings = _configService.GetSettings();
        var url = HttpUtility.BuildUrl(settings, pathOverride);

        // Create request message
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpUtility.GetHttpMethod(settings.DefaultMethod)
        };

        // Add authentication
        AddAuthentication(request, settings);

        // Add headers
        AddHeaders(request, settings);

        // Execute request
        var response = await _httpClient.SendAsync(request);

        return response;
    }

    public async Task ExecuteAndDisplayAsync(string? pathOverride = null, bool saveOutput = false)
    {
        var settings = _configService.GetSettings();
        StringBuilder? outputBuilder = saveOutput ? new StringBuilder() : null;

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== Executing HTTP Request ===\n");
            Console.ResetColor();

            if (settings.ShowProgress)
            {
                Console.WriteLine("Sending request...");
            }

            var response = await ExecuteRequestAsync(pathOverride);

            // Display and capture status
            var statusLine = $"Status: {(int)response.StatusCode} {response.ReasonPhrase}";
            Console.ForegroundColor = response.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(statusLine);
            Console.ResetColor();
            Console.WriteLine();

            outputBuilder?.AppendLine(statusLine);
            outputBuilder?.AppendLine();

            // Display and capture headers if configured
            if (settings.ShowHeaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Response Headers:");
                Console.ResetColor();
                outputBuilder?.AppendLine("Response Headers:");

                foreach (var header in response.Headers)
                {
                    var headerLine = $"  {header.Key}: {string.Join(", ", header.Value)}";
                    Console.WriteLine(headerLine);
                    outputBuilder?.AppendLine(headerLine);
                }
                if (response.Content.Headers != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        var headerLine = $"  {header.Key}: {string.Join(", ", header.Value)}";
                        Console.WriteLine(headerLine);
                        outputBuilder?.AppendLine(headerLine);
                    }
                }
                Console.WriteLine();
                outputBuilder?.AppendLine();
            }

            // Display and capture response body
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Response Body:");
            Console.ResetColor();
            outputBuilder?.AppendLine("Response Body:");

            var content = await response.Content.ReadAsStringAsync();

            // Try to format as JSON if it's JSON content
            var contentType = response.Content.Headers.ContentType?.MediaType;
            string displayContent = content ?? string.Empty;
            if (contentType is not null && contentType.Contains("json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(displayContent))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(displayContent);
                    displayContent = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                }
                catch
                {
                    // If JSON parsing fails, displayContent already contains the raw content
                }
            }

            Console.WriteLine(displayContent);
            Console.WriteLine();

            outputBuilder?.AppendLine(displayContent);
            outputBuilder?.AppendLine();

            // Save output if requested
            if (saveOutput && outputBuilder != null)
            {
                SaveOutputToFile(outputBuilder.ToString());
            }
        }
        catch (HttpRequestException ex)
        {
            if (settings.ShowError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                Console.ResetColor();
            }
        }
        catch (TaskCanceledException ex)
        {
            if (settings.ShowError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Request Timeout: {ex.Message}");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            if (settings.ShowError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    private void AddAuthentication(HttpRequestMessage request, Settings settings)
    {
        switch (settings.AuthType)
        {
            case AuthType.BearerToken:
            case AuthType.JwtBearer:
                if (!string.IsNullOrWhiteSpace(settings.AuthToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.AuthToken);
                }
                break;

            case AuthType.BasicAuth:
                if (!string.IsNullOrWhiteSpace(settings.BasicAuthUsername))
                {
                    var credentials = settings.BasicAuthUsername;
                    if (!string.IsNullOrWhiteSpace(settings.BasicAuthPassword))
                    {
                        credentials += ":" + settings.BasicAuthPassword;
                    }
                    var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
                }
                break;
        }
    }

    private void AddHeaders(HttpRequestMessage request, Settings settings)
    {
        foreach (var header in settings.DefaultHeaders)
        {
            // Try to add as a request header, if it fails, it might be a content header
            try
            {
                request.Headers.Add(header.Key, header.Value);
            }
            catch
            {
                // Some headers can only be set on content, skip for now
            }
        }
    }

    private void SaveOutputToFile(string output)
    {
        HttpUtility.SaveToFile(output, "responses", "response", "Response saved to");
    }
}
