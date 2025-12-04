namespace CoolCurl.Infrastructure;

public interface IAiClient
{
    Task<string?> GenerateCurlCommandAsync(string userPrompt);
    Task<string?> DebugHttpErrorAsync(string errorMessage, string? requestDetails = null);
}
