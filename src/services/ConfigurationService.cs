using System.Text.Json;
using CoolCurl.Models;

namespace CoolCurl.Services;

public class ConfigurationService
{
    private readonly string _configDirectory;
    private readonly string _configFilePath;

    public ConfigurationService()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _configDirectory = Path.Combine(homeDirectory, ".cool-curl");
        _configFilePath = Path.Combine(_configDirectory, ".config");
        EnsureConfigDirectoryAndFileExist();
    }

    private void EnsureConfigDirectoryAndFileExist()
    {
        if (!Directory.Exists(_configDirectory))
        {
            Directory.CreateDirectory(_configDirectory);
        }

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
        Console.WriteLine("Query Parameters:");
        Console.WriteLine("Do you want to add default query parameters? (y/n) [default: n]: ");
        var addQueryParams = Console.ReadLine()?.ToLower();

        if (addQueryParams == "y")
        {
            Console.WriteLine("Enter query parameters (leave key empty to finish):");
            while (true)
            {
                Console.Write("Parameter name: ");
                var paramName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(paramName))
                    break;

                Console.Write($"Value for {paramName}: ");
                var paramValue = Console.ReadLine() ?? "";
                settings.QueryParameters[paramName] = paramValue;
            }
        }

        Console.WriteLine();
        Console.Write("Enter an initial path (leave empty to skip): ");
        var initialPath = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            settings.RecentPaths.Add(initialPath);
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("AI Configuration (Optional)");
        Console.ResetColor();
        Console.WriteLine("Choose an AI provider for curl generation and debugging (or skip):");
        Console.WriteLine("  1. Google Gemini (free tier available)");
        Console.WriteLine("  2. OpenAI (gpt-4o-mini - cheapest option)");
        Console.WriteLine("  3. Skip AI features");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Warning: API keys will be stored in plain text!");
        Console.ResetColor();
        Console.Write("Select AI provider (1-3) [default: 3]: ");
        var aiProviderInput = Console.ReadLine();

        if (aiProviderInput == "1")
        {
            Console.Write("\nGemini API Key: ");
            var geminiKey = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(geminiKey))
            {
                settings.GeminiApiKey = geminiKey.Trim();

                Console.Write("\nAllow AI to help debug HTTP errors? (y/n) [default: n]: ");
                var allowDebugging = Console.ReadLine()?.ToLower();
                settings.AllowAiDebugging = allowDebugging == "y";
            }
        }
        else if (aiProviderInput == "2")
        {
            Console.Write("\nOpenAI API Key: ");
            var openAiKey = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(openAiKey))
            {
                settings.OpenAiApiKey = openAiKey.Trim();

                Console.Write("\nAllow AI to help debug HTTP errors? (y/n) [default: n]: ");
                var allowDebugging = Console.ReadLine()?.ToLower();
                settings.AllowAiDebugging = allowDebugging == "y";
            }
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

    public virtual Settings GetSettings()
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

        settings.QueryParameters = new Dictionary<string, string>();

        SaveSettings(settings);
    }

    public bool ValidateSettings(out string errorMessage)
    {
        var settings = LoadSettings();
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            errorMessage = "Base URL is not configured.";
            return false;
        }

        if (settings.RecentPaths.Count == 0)
        {
            errorMessage = "No paths available. Use -p <path> to add a path.";
            return false;
        }

        if (settings.AuthType == AuthType.BasicAuth)
        {
            if (string.IsNullOrWhiteSpace(settings.BasicAuthUsername))
            {
                errorMessage = "Basic Auth is configured but username is missing.";
                return false;
            }
        }
        else if (settings.AuthType != AuthType.None)
        {
            if (string.IsNullOrWhiteSpace(settings.AuthToken))
            {
                errorMessage = $"{settings.AuthType} is configured but auth token is missing.";
                return false;
            }
        }

        return true;
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
        PrintRow("Gemini API Key", MaskValue(settings.GeminiApiKey), labelWidth);
        PrintRow("OpenAI API Key", MaskValue(settings.OpenAiApiKey), labelWidth);
        PrintRow("AI Debugging", settings.AllowAiDebugging ? "Enabled" : "Disabled", labelWidth);

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
        Console.WriteLine("Query Parameters:");
        Console.ResetColor();
        if (settings.QueryParameters.Count > 0)
        {
            foreach (var param in settings.QueryParameters)
            {
                Console.WriteLine($"  {param.Key}: {param.Value}");
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
