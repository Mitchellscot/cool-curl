using System.Text.Json;
using CoolCurl.Models;

namespace CoolCurl.Services;

public class ConfigurationService
{
    private const string ConfigFileName = ".cool_curl_config";
    private readonly string _configFilePath;

    public ConfigurationService()
    {
        _configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ConfigFileName
        );
        EnsureConfigFileExists();
    }

    private void EnsureConfigFileExists()
    {
        if (!File.Exists(_configFilePath) || !IsValidConfig())
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to CoolCurl! Let's set up your configuration.\n");
            Console.ResetColor();
            var settings = PromptUserForSettings();
            SaveSettings(settings);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nConfiguration saved to: {_configFilePath}\n");
            Console.ResetColor();
        }
    }

    private bool IsValidConfig()
    {
        try
        {
            var json = File.ReadAllText(_configFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }
            var settings = JsonSerializer.Deserialize<Settings>(json);
            return settings != null;
        }
        catch
        {
            return false;
        }
    }

    private Settings PromptUserForSettings()
    {
        var settings = new Settings();

        string? baseUrl = null;
        while (string.IsNullOrWhiteSpace(baseUrl))
        {
            Console.Write("Base URL (required, or type 'exit' to quit): ");
            baseUrl = Console.ReadLine();
            if (baseUrl?.ToLower() == "exit")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exiting setup...");
                Console.ResetColor();
                Environment.Exit(0);
            }
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Base URL is required. Please enter a value.");
                Console.ResetColor();
            }
        }
        settings.BaseUrl = baseUrl;
        Console.WriteLine($"Base URL set to: {baseUrl}\n");

        Console.Write("Default HTTP method (e.g., GET, POST) [leave empty for GET]: ");
        var defaultMethod = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(defaultMethod))
            settings.DefaultMethod = defaultMethod;
        else
            settings.DefaultMethod = "GET";

        Console.WriteLine("\nAuth type options:");
        Console.WriteLine("  1. None (default)");
        Console.WriteLine("  2. Bearer Token");
        Console.WriteLine("  3. Basic Auth");
        Console.WriteLine("  4. JWT Bearer");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Warning: Auth credentials will be stored in plain text!");
        Console.ResetColor();
        Console.Write("Select auth type (1-4) [default: 1]: ");
        var authTypeInput = Console.ReadLine();

        settings.AuthType = authTypeInput switch
        {
            "2" => AuthType.BearerToken,
            "3" => AuthType.BasicAuth,
            "4" => AuthType.JwtBearer,
            _ => AuthType.None
        };

        if (settings.AuthType == AuthType.BasicAuth)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(username))
                settings.BasicAuthUsername = username;

            Console.Write("Password: ");
            var password = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(password))
                settings.BasicAuthPassword = password;
        }
        else if (settings.AuthType != AuthType.None)
        {
            Console.Write("Auth token (leave empty to skip): ");
            var authToken = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(authToken))
                settings.AuthToken = authToken;
        }

        Console.Write("Follow redirects? (y/n) [default: y]: ");
        var followRedirects = Console.ReadLine()?.ToLower();
        settings.FollowRedirects = followRedirects != "n";

        Console.Write("Show progress? (y/n) [default: n]: ");
        var showProgress = Console.ReadLine()?.ToLower();
        settings.ShowProgress = showProgress == "y";

        Console.Write("Show error messages? (y/n) [default: y]: ");
        var showError = Console.ReadLine()?.ToLower();
        settings.ShowError = showError != "n";

        Console.Write("Show headers in response? (y/n) [default: y]: ");
        var showHeaders = Console.ReadLine()?.ToLower();
        settings.ShowHeaders = showHeaders != "n";

        Console.Write("Max time in seconds (leave empty to skip): ");
        var maxTimeInput = Console.ReadLine();
        if (int.TryParse(maxTimeInput, out int maxTime))
            settings.MaxTimeSeconds = maxTime;

        Console.WriteLine("\nDefault headers:");
        foreach (var header in settings.DefaultHeaders)
        {
            Console.WriteLine($"  {header.Key}: {header.Value}");
        }
        Console.Write("Are these default headers OK? (y/n) [default: y]: ");
        var headersOk = Console.ReadLine()?.ToLower();

        if (headersOk == "n")
        {
            settings.DefaultHeaders.Clear();
            Console.WriteLine("\nEnter custom headers (leave key empty to finish):");
            while (true)
            {
                Console.Write("Header name: ");
                var headerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(headerName))
                    break;

                Console.Write($"Value for {headerName}: ");
                var headerValue = Console.ReadLine() ?? "";
                settings.DefaultHeaders[headerName] = headerValue;
            }
        }

        Console.WriteLine();
        Console.Write("Enter an initial path (leave empty to skip): ");
        var initialPath = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            settings.RecentPaths.Add(initialPath);
        }

        return settings;
    }

    private Settings LoadSettings()
    {
        try
        {
            var json = File.ReadAllText(_configFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Settings();
            }
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }
        catch
        {
            return new Settings();
        }
    }

    private void SaveSettings(Settings settings)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(_configFilePath, json);
    }

    private Dictionary<string, string> LoadConfig()
    {
        try
        {
            var json = File.ReadAllText(_configFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, string>();
            }
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private void SaveConfig(Dictionary<string, string> config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_configFilePath, json);
    }

    public Settings GetSettings()
    {
        return LoadSettings();
    }

    public void UpdateSettings(Settings settings)
    {
        SaveSettings(settings);
    }

    public void AddRecentPath(string path)
    {
        var settings = LoadSettings();

        settings.RecentPaths.Remove(path);

        settings.RecentPaths.Insert(0, path);

        SaveSettings(settings);
    }

    public void DisplaySettings()
    {
        var settings = LoadSettings();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== CoolCurl Settings ===\n");
        Console.ResetColor();

        const int labelWidth = 25;

        PrintRow("Base URL", settings.BaseUrl ?? "(not set)", labelWidth);
        PrintRow("Default Method", settings.DefaultMethod ?? "(not set)", labelWidth);
        PrintRow("Auth Type", settings.AuthType.ToString(), labelWidth);

        if (settings.AuthType == AuthType.BasicAuth)
        {
            PrintRow("Username", settings.BasicAuthUsername ?? "(not set)", labelWidth);
            PrintRow("Password", MaskValue(settings.BasicAuthPassword), labelWidth);
        }
        else if (settings.AuthType != AuthType.None)
        {
            PrintRow("Auth Token", MaskValue(settings.AuthToken), labelWidth);
        }

        PrintRow("Follow Redirects", settings.FollowRedirects ? "Yes" : "No", labelWidth);
        PrintRow("Show Progress", settings.ShowProgress ? "Yes" : "No", labelWidth);
        PrintRow("Show Error", settings.ShowError ? "Yes" : "No", labelWidth);
        PrintRow("Show Headers", settings.ShowHeaders ? "Yes" : "No", labelWidth);
        PrintRow("Max Time (seconds)", settings.MaxTimeSeconds?.ToString() ?? "(not set)", labelWidth);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Default Headers:");
        Console.ResetColor();
        if (settings.DefaultHeaders.Count > 0)
        {
            foreach (var header in settings.DefaultHeaders)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
        }
        else
        {
            Console.WriteLine("  (none)");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Recent Paths:");
        Console.ResetColor();
        if (settings.RecentPaths.Count > 0)
        {
            for (int i = 0; i < settings.RecentPaths.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {settings.RecentPaths[i]}");
            }
        }
        else
        {
            Console.WriteLine("  (none)");
        }

        Console.WriteLine();
    }

    private static void PrintRow(string label, string value, int labelWidth)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{label.PadRight(labelWidth)}: ");
        Console.ResetColor();
        Console.WriteLine(value);
    }

    private static string MaskValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "(not set)";

        if (value.Length <= 4)
            return "****";

        return value.Substring(0, 4) + new string('*', value.Length - 4);
    }

    public string GetConfigFilePath() => _configFilePath;
}
