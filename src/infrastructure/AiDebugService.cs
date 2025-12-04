using CoolCurl.Models;
using CoolCurl.Services;

namespace CoolCurl.Infrastructure;

public class AiDebugService
{
    private readonly IAiClient _aiClient;
    private readonly ConfigurationService _configService;

    public AiDebugService(IAiClient aiClient, ConfigurationService configService)
    {
        _aiClient = aiClient;
        _configService = configService;
    }

    public async Task<string?> DebugHttpErrorAsync(Exception exception, string? url = null, string? method = null)
    {
        var settings = _configService.GetSettings();

        if (!settings.AllowAiDebugging)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(settings.OpenAiApiKey) && string.IsNullOrWhiteSpace(settings.GeminiApiKey))
        {
            return null;
        }

        var requestDetails = BuildRequestDetails(url, method, settings);
        var errorMessage = BuildErrorMessage(exception);

        try
        {
            return await _aiClient.DebugHttpErrorAsync(errorMessage, requestDetails);
        }
        catch
        {
            return null;
        }
    }

    private string BuildErrorMessage(Exception exception)
    {
        var message = exception.Message;

        if (exception is HttpRequestException httpEx)
        {
            message = $"HTTP Request Exception: {httpEx.Message}";
            if (httpEx.StatusCode.HasValue)
            {
                message += $"\nStatus Code: {(int)httpEx.StatusCode} ({httpEx.StatusCode})";
            }
        }
        else if (exception is TaskCanceledException)
        {
            message = "Request Timeout: The request took too long to complete";
        }

        if (exception.InnerException != null)
        {
            message += $"\nInner Exception: {exception.InnerException.Message}";
        }

        return message;
    }

    private string BuildRequestDetails(string? url, string? method, Settings settings)
    {
        var details = new List<string>();

        if (!string.IsNullOrWhiteSpace(url))
        {
            details.Add($"URL: {url}");
        }

        if (!string.IsNullOrWhiteSpace(method))
        {
            details.Add($"Method: {method}");
        }

        details.Add($"Auth Type: {settings.AuthType}");

        if (settings.MaxTimeSeconds.HasValue)
        {
            details.Add($"Timeout: {settings.MaxTimeSeconds} seconds");
        }

        if (settings.DefaultHeaders.Count > 0)
        {
            details.Add($"Headers: {string.Join(", ", settings.DefaultHeaders.Keys)}");
        }

        return string.Join("\n", details);
    }

    public async Task DisplayDebugSuggestionsAsync(Exception exception, string? url = null, string? method = null)
    {
        var suggestions = await DebugHttpErrorAsync(exception, url, method);

        if (!string.IsNullOrWhiteSpace(suggestions))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=== AI Debug Suggestions ===");

            Console.WriteLine();
            Console.WriteLine(suggestions);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
