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
        var url = BuildUrl(settings, pathOverride);

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

    private string BuildUrl(Settings settings, string? pathOverride)
    {
        var baseUrl = settings.BaseUrl?.TrimEnd('/') ?? "";

        // Use override path if provided, otherwise use most recent path
        var path = pathOverride;
        if (string.IsNullOrWhiteSpace(path) && settings.RecentPaths.Count > 0)
        {
            path = settings.RecentPaths[0];
        }

        // Ensure path starts with /
        if (!string.IsNullOrWhiteSpace(path) && !path.StartsWith("/"))
        {
            path = "/" + path;
        }

        return baseUrl + (path ?? "");
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

    public void PrintCurlCommand(string? pathOverride = null)
    {
        var command = BuildCurlCommand(pathOverride);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Generated cURL Command ===\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(command);
        Console.ResetColor();
        Console.WriteLine();
    }
}
