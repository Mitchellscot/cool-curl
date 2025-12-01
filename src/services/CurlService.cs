using CoolCurl.Models;
using System.Text;

namespace CoolCurl.Services;

public class CurlService
{
    private readonly ConfigurationService _configService;

    public CurlService(ConfigurationService configService)
    {
        _configService = configService;
    }

    public string BuildCurlCommand(string? pathOverride = null)
    {
        var settings = _configService.GetSettings();
        var curl = new StringBuilder("curl");

        // Build the URL
        var url = HttpUtility.BuildUrl(settings, pathOverride);

        // Add HTTP method
        if (!string.IsNullOrWhiteSpace(settings.DefaultMethod) && settings.DefaultMethod.ToUpper() != "GET")
        {
            curl.Append($" -X {settings.DefaultMethod.ToUpper()}");
        }

        // Add authentication
        AddAuthentication(curl, settings);

        // Add headers
        AddHeaders(curl, settings);

        // Add other options
        if (settings.FollowRedirects)
        {
            curl.Append(" -L");
        }

        if (settings.ShowProgress)
        {
            curl.Append(" --progress-bar");
        }

        if (settings.ShowError)
        {
            curl.Append(" --show-error");
        }

        if (settings.ShowHeaders)
        {
            curl.Append(" -i");
        }

        if (settings.MaxTimeSeconds.HasValue)
        {
            curl.Append($" --max-time {settings.MaxTimeSeconds.Value}");
        }

        // Add URL at the end
        curl.Append($" \"{url}\"");

        return curl.ToString();
    }

    private void AddAuthentication(StringBuilder curl, Settings settings)
    {
        switch (settings.AuthType)
        {
            case AuthType.BearerToken:
                if (!string.IsNullOrWhiteSpace(settings.AuthToken))
                {
                    curl.Append($" -H \"Authorization: Bearer {settings.AuthToken}\"");
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
                    curl.Append($" -u \"{credentials}\"");
                }
                break;

            case AuthType.JwtBearer:
                if (!string.IsNullOrWhiteSpace(settings.AuthToken))
                {
                    curl.Append($" -H \"Authorization: Bearer {settings.AuthToken}\"");
                }
                break;
        }
    }

    private void AddHeaders(StringBuilder curl, Settings settings)
    {
        foreach (var header in settings.DefaultHeaders)
        {
            curl.Append($" -H \"{header.Key}: {header.Value}\"");
        }
    }

    public void PrintCurlCommand(string? pathOverride = null, bool saveToDisk = false)
    {
        var command = BuildCurlCommand(pathOverride);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Generated cURL Command ===\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(command);
        Console.ResetColor();
        Console.WriteLine();

        // Save to disk if requested
        if (saveToDisk)
        {
            SaveCurlCommand(command);
        }
    }

    public void SaveCurlCommandOnly(string? pathOverride = null)
    {
        var command = BuildCurlCommand(pathOverride);
        SaveCurlCommand(command);
    }

    private void SaveCurlCommand(string command)
    {
        HttpUtility.SaveToFile(command, "curl-commands", "curl", "Curl command saved to");
    }
}
