using CoolCurl.Models;

namespace CoolCurl.Services;

public static class HttpUtility
{
    public static string BuildUrl(Settings settings, string? pathOverride = null)
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

        var url = baseUrl + (path ?? "");

        // Add query parameters if any
        if (settings.QueryParameters.Count > 0)
        {
            var queryString = string.Join("&", settings.QueryParameters.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            url += (url.Contains('?') ? "&" : "?") + queryString;
        }

        return url;
    }

    public static HttpMethod GetHttpMethod(string? method)
    {
        return method?.ToUpper() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            _ => HttpMethod.Get
        };
    }

    public static void SaveToFile(string content, string subdirectory, string filePrefix, string successMessage)
    {
        try
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outputDirectory = Path.Combine(homeDirectory, ".cool-curl", subdirectory);

            // Create directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Generate filename with timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"{filePrefix}_{timestamp}.txt";
            var filePath = Path.Combine(outputDirectory, filename);

            // Write content to file
            File.WriteAllText(filePath, content);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{successMessage}: {filePath}");
            Console.ResetColor();
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error saving file: {ex.Message}");
            Console.ResetColor();
        }
    }
}
