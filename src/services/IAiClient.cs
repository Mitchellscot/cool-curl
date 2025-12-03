namespace CoolCurl.Services;

public interface IAiClient
{
    Task<string?> DebugHttpErrorAsync(string errorMessage, string? requestDetails = null);
}
