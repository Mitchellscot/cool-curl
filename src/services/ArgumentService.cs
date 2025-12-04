using System.IO;

namespace CoolCurl.Services;

public class ArgumentService
{
    public bool ShowHelp { get; private set; }
    public bool ShowVersion { get; private set; }
    public bool ResetConfig { get; private set; }
    public bool ShowSettings { get; private set; }
    public bool ExecuteRequest { get; private set; }
    public bool SaveCurl { get; private set; }
    public bool SaveOutput { get; private set; }
    public bool ChangeMethod { get; private set; }
    public bool ChangeUrl { get; private set; }
    public bool ChangeAuth { get; private set; }
    public bool ChangeToken { get; private set; }
    public bool ChangeUsername { get; private set; }
    public bool ChangePassword { get; private set; }
    public bool ChangeFollowRedirects { get; private set; }
    public bool ChangeShowHeaders { get; private set; }
    public bool ChangeShowError { get; private set; }
    public bool ChangeShowProgress { get; private set; }
    public bool ChangeMaxTimeout { get; private set; }
    public bool ChangeDefaultHeaders { get; private set; }
    public bool ChangeQueryParameters { get; private set; }
    public bool ChangeGeminiApiKey { get; private set; }
    public bool ChangeOpenAiApiKey { get; private set; }
    public bool ChangeAiDebugging { get; private set; }
    public bool UseAiPrompt { get; private set; }
    public string? AiPrompt { get; private set; }
    public string? Path { get; private set; }

    public ArgumentService(string[] args)
    {
        ParseArguments(args);
    }

    private void ParseArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-h":
                case "--help":
                    ShowHelp = true;
                    break;
                case "-v":
                case "--version":
                    ShowVersion = true;
                    break;
                case "-r":
                case "--reset":
                    ResetConfig = true;
                    break;
                case "-c":
                case "--config":
                    ShowSettings = true;
                    break;
                case "-e":
                case "--execute":
                    ExecuteRequest = true;
                    break;
                case "-s":
                case "--save":
                    SaveCurl = true;
                    break;
                case "-o":
                case "--output":
                    SaveOutput = true;
                    break;
                case "-m":
                case "--method":
                    ChangeMethod = true;
                    break;
                case "-u":
                case "--url":
                    ChangeUrl = true;
                    break;
                case "-a":
                case "--auth":
                    ChangeAuth = true;
                    break;
                case "-t":
                case "--token":
                    ChangeToken = true;
                    break;
                case "-un":
                case "--username":
                    ChangeUsername = true;
                    break;
                case "-pw":
                case "--password":
                    ChangePassword = true;
                    break;
                case "-f":
                case "--follow":
                    ChangeFollowRedirects = true;
                    break;
                case "-sh":
                case "--show-headers":
                    ChangeShowHeaders = true;
                    break;
                case "-se":
                case "--show-errors":
                    ChangeShowError = true;
                    break;
                case "-sp":
                case "--show-progress":
                    ChangeShowProgress = true;
                    break;
                case "-mt":
                case "--max-timeout":
                    ChangeMaxTimeout = true;
                    break;
                case "-dh":
                case "--default-headers":
                    ChangeDefaultHeaders = true;
                    break;
                case "-qp":
                case "--query-parameters":
                    ChangeQueryParameters = true;
                    break;
                case "-gk":
                case "--gemini-key":
                    ChangeGeminiApiKey = true;
                    break;
                case "-ok":
                case "--openai-key":
                    ChangeOpenAiApiKey = true;
                    break;
                case "-ad":
                case "--ai-debugging":
                    ChangeAiDebugging = true;
                    break;
                case "-ai":
                case "--ai-prompt":
                    if (i + 1 < args.Length)
                    {
                        UseAiPrompt = true;
                        AiPrompt = args[i + 1];
                        i++;
                    }
                    break;
                case "-p":
                case "--path":
                    if (i + 1 < args.Length)
                    {
                        Path = args[i + 1];
                        i++;
                    }
                    break;
            }
        }
    }

    public void HandleResetConfig()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configDirectory = System.IO.Path.Combine(homeDirectory, ".cool-curl");
        var configPath = System.IO.Path.Combine(configDirectory, ".config");

        if (System.IO.File.Exists(configPath))
        {
            System.IO.File.Delete(configPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Configuration deleted successfully.");
            Console.ResetColor();
        }
    }

    public void HandleMethodChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change HTTP Method ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current method: {currentSettings.DefaultMethod ?? "GET"}");
        Console.WriteLine($"\nAvailable methods: {string.Join(", ", CoolCurl.Models.HttpMethods.ValidMethods)}");

        string? newMethod = null;
        while (newMethod == null)
        {
            Console.Write("\nEnter new HTTP method: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Method cannot be empty.");
                Console.ResetColor();
                continue;
            }

            var normalized = CoolCurl.Models.HttpMethods.Normalize(input);
            if (CoolCurl.Models.HttpMethods.IsValid(input))
            {
                newMethod = normalized;
                currentSettings.DefaultMethod = newMethod;
                configService.UpdateSettings(currentSettings);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nHTTP method changed to: {newMethod}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid method. Please choose from: {string.Join(", ", CoolCurl.Models.HttpMethods.ValidMethods)}");
                Console.ResetColor();
            }
        }
    }

    public void HandleUrlChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Base URL ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current URL: {currentSettings.BaseUrl ?? "(not set)"}");

        Console.Write("\nEnter new base URL: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("URL cannot be empty.");
            Console.ResetColor();
            return;
        }

        currentSettings.BaseUrl = input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nBase URL changed to: {currentSettings.BaseUrl}");
        Console.ResetColor();
    }

    public void HandleAuthChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Authentication Type ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current auth type: {currentSettings.AuthType}");
        Console.WriteLine("\nAvailable types:");
        Console.WriteLine("0 - None");
        Console.WriteLine("1 - BearerToken");
        Console.WriteLine("2 - BasicAuth");
        Console.WriteLine("3 - JwtBearer");

        Console.Write("\nEnter auth type number: ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out int authValue) && Enum.IsDefined(typeof(CoolCurl.Models.AuthType), authValue))
        {
            currentSettings.AuthType = (CoolCurl.Models.AuthType)authValue;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAuthentication type changed to: {currentSettings.AuthType}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid auth type. Please enter a number between 0 and 3.");
            Console.ResetColor();
        }
    }

    public void HandleTokenChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Auth Token ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        var maskedToken = string.IsNullOrEmpty(currentSettings.AuthToken) ? "(not set)" : "***" + currentSettings.AuthToken[^4..];
        Console.WriteLine($"Current token: {maskedToken}");

        Console.Write("\nEnter new auth token (leave empty to clear): ");
        var input = Console.ReadLine();

        currentSettings.AuthToken = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(currentSettings.AuthToken == null ? "\nAuth token cleared." : "\nAuth token updated.");
        Console.ResetColor();
    }

    public void HandleUsernameChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Basic Auth Username ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current username: {currentSettings.BasicAuthUsername ?? "(not set)"}");

        Console.Write("\nEnter new username (leave empty to clear): ");
        var input = Console.ReadLine();

        currentSettings.BasicAuthUsername = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(currentSettings.BasicAuthUsername == null ? "\nUsername cleared." : $"\nUsername changed to: {currentSettings.BasicAuthUsername}");
        Console.ResetColor();
    }

    public void HandlePasswordChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Basic Auth Password ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        var maskedPassword = string.IsNullOrEmpty(currentSettings.BasicAuthPassword) ? "(not set)" : "******";
        Console.WriteLine($"Current password: {maskedPassword}");

        Console.Write("\nEnter new password (leave empty to clear): ");
        var input = Console.ReadLine();

        currentSettings.BasicAuthPassword = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(currentSettings.BasicAuthPassword == null ? "\nPassword cleared." : "\nPassword updated.");
        Console.ResetColor();
    }

    public void HandleFollowRedirectsChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Follow Redirects ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current setting: {currentSettings.FollowRedirects}");

        Console.Write("\nFollow redirects? (true/false): ");
        var input = Console.ReadLine();

        if (bool.TryParse(input, out bool value))
        {
            currentSettings.FollowRedirects = value;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nFollow redirects changed to: {value}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
            Console.ResetColor();
        }
    }

    public void HandleShowHeadersChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Show Headers ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current setting: {currentSettings.ShowHeaders}");

        Console.Write("\nShow headers? (true/false): ");
        var input = Console.ReadLine();

        if (bool.TryParse(input, out bool value))
        {
            currentSettings.ShowHeaders = value;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nShow headers changed to: {value}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
            Console.ResetColor();
        }
    }

    public void HandleShowErrorChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Show Error ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current setting: {currentSettings.ShowError}");

        Console.Write("\nShow errors? (true/false): ");
        var input = Console.ReadLine();

        if (bool.TryParse(input, out bool value))
        {
            currentSettings.ShowError = value;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nShow error changed to: {value}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
            Console.ResetColor();
        }
    }

    public void HandleShowProgressChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Show Progress ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current setting: {currentSettings.ShowProgress}");

        Console.Write("\nShow progress? (true/false): ");
        var input = Console.ReadLine();

        if (bool.TryParse(input, out bool value))
        {
            currentSettings.ShowProgress = value;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nShow progress changed to: {value}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
            Console.ResetColor();
        }
    }

    public void HandleMaxTimeoutChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Max Timeout ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current timeout: {currentSettings.MaxTimeSeconds} seconds");

        Console.Write("\nEnter new max timeout in seconds: ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out int seconds) && seconds > 0)
        {
            currentSettings.MaxTimeSeconds = seconds;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nMax timeout changed to: {seconds} seconds");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter a positive number.");
            Console.ResetColor();
        }
    }

    public void HandleDefaultHeadersChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Default Headers ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine("Current headers:");
        if (currentSettings.DefaultHeaders != null && currentSettings.DefaultHeaders.Count > 0)
        {
            foreach (var header in currentSettings.DefaultHeaders)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
        }
        else
        {
            Console.WriteLine("  (none)");
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1 - Add/Update header");
        Console.WriteLine("2 - Remove header");
        Console.WriteLine("3 - Clear all headers");
        Console.Write("\nSelect option: ");
        var option = Console.ReadLine();

        switch (option)
        {
            case "1":
                Console.Write("Enter header name: ");
                var name = Console.ReadLine();
                Console.Write("Enter header value: ");
                var value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (currentSettings.DefaultHeaders == null)
                        currentSettings.DefaultHeaders = new Dictionary<string, string>();

                    currentSettings.DefaultHeaders[name.Trim()] = value?.Trim() ?? string.Empty;
                    configService.UpdateSettings(currentSettings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nHeader '{name.Trim()}' updated.");
                    Console.ResetColor();
                }
                break;

            case "2":
                Console.Write("Enter header name to remove: ");
                var removeKey = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(removeKey) && currentSettings.DefaultHeaders?.Remove(removeKey.Trim()) == true)
                {
                    configService.UpdateSettings(currentSettings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nHeader '{removeKey.Trim()}' removed.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Header not found.");
                    Console.ResetColor();
                }
                break;

            case "3":
                currentSettings.DefaultHeaders = new Dictionary<string, string>();
                configService.UpdateSettings(currentSettings);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAll headers cleared.");
                Console.ResetColor();
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid option.");
                Console.ResetColor();
                break;
        }
    }

    public void HandleQueryParametersChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Query Parameters ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine("Current query parameters:");
        if (currentSettings.QueryParameters != null && currentSettings.QueryParameters.Count > 0)
        {
            foreach (var param in currentSettings.QueryParameters)
            {
                Console.WriteLine($"  {param.Key}={param.Value}");
            }
        }
        else
        {
            Console.WriteLine("  (none)");
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1 - Add/Update parameter");
        Console.WriteLine("2 - Remove parameter");
        Console.WriteLine("3 - Clear all parameters");
        Console.Write("\nSelect option: ");
        var option = Console.ReadLine();

        switch (option)
        {
            case "1":
                Console.Write("Enter parameter name: ");
                var name = Console.ReadLine();
                Console.Write("Enter parameter value: ");
                var value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (currentSettings.QueryParameters == null)
                        currentSettings.QueryParameters = new Dictionary<string, string>();

                    currentSettings.QueryParameters[name.Trim()] = value?.Trim() ?? string.Empty;
                    configService.UpdateSettings(currentSettings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nParameter '{name.Trim()}' updated.");
                    Console.ResetColor();
                }
                break;

            case "2":
                Console.Write("Enter parameter name to remove: ");
                var removeKey = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(removeKey) && currentSettings.QueryParameters?.Remove(removeKey.Trim()) == true)
                {
                    configService.UpdateSettings(currentSettings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nParameter '{removeKey.Trim()}' removed.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Parameter not found.");
                    Console.ResetColor();
                }
                break;

            case "3":
                currentSettings.QueryParameters = new Dictionary<string, string>();
                configService.UpdateSettings(currentSettings);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAll query parameters cleared.");
                Console.ResetColor();
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                Console.ResetColor();
                break;
        }
    }

    public void HandleGeminiApiKeyChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change Gemini API Key ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        var maskedKey = string.IsNullOrEmpty(currentSettings.GeminiApiKey) ? "(not set)" : "***" + currentSettings.GeminiApiKey[^4..];
        Console.WriteLine($"Current Gemini API Key: {maskedKey}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Warning: API key will be stored in plain text!");
        Console.ResetColor();
        Console.Write("\nEnter new Gemini API key (leave empty to clear): ");
        var input = Console.ReadLine();

        currentSettings.GeminiApiKey = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(currentSettings.GeminiApiKey == null ? "\nGemini API key cleared." : "\nGemini API key updated.");
        Console.ResetColor();
    }

    public void HandleOpenAiApiKeyChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change OpenAI API Key ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        var maskedKey = string.IsNullOrEmpty(currentSettings.OpenAiApiKey) ? "(not set)" : "***" + currentSettings.OpenAiApiKey[^4..];
        Console.WriteLine($"Current OpenAI API Key: {maskedKey}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Warning: API key will be stored in plain text!");
        Console.ResetColor();
        Console.Write("\nEnter new OpenAI API key (leave empty to clear): ");
        var input = Console.ReadLine();

        currentSettings.OpenAiApiKey = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        configService.UpdateSettings(currentSettings);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(currentSettings.OpenAiApiKey == null ? "\nOpenAI API key cleared." : "\nOpenAI API key updated.");
        Console.ResetColor();
    }

    public void HandleAiDebuggingChange(ConfigurationService configService)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Change AI Debugging Setting ===\n");
        Console.ResetColor();

        var currentSettings = configService.GetSettings();
        Console.WriteLine($"Current setting: {(currentSettings.AllowAiDebugging ? "Enabled" : "Disabled")}");
        Console.WriteLine("\nWhen enabled, AI will analyze HTTP errors and suggest fixes.");
        Console.WriteLine("This requires a valid Gemini or OpenAI API key.");

        Console.Write("\nEnable AI debugging? (true/false): ");
        var input = Console.ReadLine();

        if (bool.TryParse(input, out bool value))
        {
            currentSettings.AllowAiDebugging = value;
            configService.UpdateSettings(currentSettings);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAI debugging {(value ? "enabled" : "disabled")}.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
            Console.ResetColor();
        }
    }
}
