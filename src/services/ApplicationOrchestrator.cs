using CoolCurl.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CoolCurl.Services;

public class ApplicationOrchestrator
{
    private readonly IConsoleWriter _console;

    public ApplicationOrchestrator(IConsoleWriter console)
    {
        _console = console;
    }

    public async Task<int> RunAsync(string[] args, IConfiguration configuration)
    {
        var version = configuration["Application:Version"] ?? "Unknown";

        var argumentService = new ArgumentService(args);

        if (argumentService.SaveOutput && !argumentService.ExecuteRequest)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine("Error: -o (--output) can only be used with -e (--execute).");
            _console.ResetColor();
            return 1;
        }

        if (argumentService.ShowHelp)
        {
            HelpService.DisplayHelp();
            return 0;
        }

        if (argumentService.ShowVersion)
        {
            _console.WriteLine($"CoolCurl version {version}");
            return 0;
        }

        if (argumentService.ResetConfig)
        {
            argumentService.HandleResetConfig();
        }

        var configService = new ConfigurationService();

        if (argumentService.ShowSettings)
        {
            configService.DisplaySettings();
            return 0;
        }

        if (argumentService.ChangeMethod)
        {
            argumentService.HandleMethodChange(configService);
            return 0;
        }

        if (argumentService.ChangeUrl)
        {
            argumentService.HandleUrlChange(configService);
            return 0;
        }

        if (argumentService.ChangeAuth)
        {
            argumentService.HandleAuthChange(configService);
            return 0;
        }

        if (argumentService.ChangeToken)
        {
            argumentService.HandleTokenChange(configService);
            return 0;
        }

        if (argumentService.ChangeUsername)
        {
            argumentService.HandleUsernameChange(configService);
            return 0;
        }

        if (argumentService.ChangePassword)
        {
            argumentService.HandlePasswordChange(configService);
            return 0;
        }

        if (argumentService.ChangeFollowRedirects)
        {
            argumentService.HandleFollowRedirectsChange(configService);
            return 0;
        }

        if (argumentService.ChangeShowHeaders)
        {
            argumentService.HandleShowHeadersChange(configService);
            return 0;
        }

        if (argumentService.ChangeShowError)
        {
            argumentService.HandleShowErrorChange(configService);
            return 0;
        }

        if (argumentService.ChangeShowProgress)
        {
            argumentService.HandleShowProgressChange(configService);
            return 0;
        }

        if (argumentService.ChangeMaxTimeout)
        {
            argumentService.HandleMaxTimeoutChange(configService);
            return 0;
        }

        if (argumentService.ChangeDefaultHeaders)
        {
            argumentService.HandleDefaultHeadersChange(configService);
            return 0;
        }

        if (argumentService.ChangeQueryParameters)
        {
            argumentService.HandleQueryParametersChange(configService);
            return 0;
        }

        if (argumentService.ChangeGeminiApiKey)
        {
            argumentService.HandleGeminiApiKeyChange(configService);
            return 0;
        }

        if (argumentService.ChangeOpenAiApiKey)
        {
            argumentService.HandleOpenAiApiKeyChange(configService);
            return 0;
        }

        if (argumentService.ChangeAiDebugging)
        {
            argumentService.HandleAiDebuggingChange(configService);
            return 0;
        }

        if (argumentService.UseAiPrompt && !string.IsNullOrWhiteSpace(argumentService.AiPrompt))
        {
            await HandleAiPromptGenerationAsync(argumentService, configService);
            return 0;
        }

        if (!string.IsNullOrWhiteSpace(argumentService.Path))
        {
            string? path = argumentService.Path;

            while (string.IsNullOrWhiteSpace(path))
            {
                _console.ForegroundColor = ConsoleColor.Yellow;
                _console.WriteLine("Path cannot be empty.");
                _console.ResetColor();
                _console.Write("Enter path: ");
                path = _console.ReadLine();
            }

            configService.AddRecentPath(path);

            _console.ForegroundColor = ConsoleColor.Green;
            _console.WriteLine($"Path '{path}' added to recent paths.");
            _console.ResetColor();

            if (argumentService.ExecuteRequest)
            {
                await ExecuteRequestWithPathAsync(path, argumentService, configService);
            }
            else
            {
                GenerateCurlCommandForPath(path, argumentService, configService);
            }
            return 0;
        }

        var settings = configService.GetSettings();
        if (settings.RecentPaths.Count > 0)
        {
            if (argumentService.ExecuteRequest)
            {
                await ExecuteDefaultRequestAsync(argumentService, configService);
            }
            else
            {
                GenerateDefaultCurlCommand(argumentService, configService);
            }
        }
        else
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.WriteLine("No recent URL paths available. Use -p <path> to add a path.");
            _console.ResetColor();
        }

        return 0;
    }

    public bool ValidateSettingsOrExit(ConfigurationService configService)
    {
        if (!configService.ValidateSettings(out string errorMessage))
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine($"Configuration error: {errorMessage}");
            _console.ResetColor();
            return false;
        }
        return true;
    }

    public async Task HandleAiPromptGenerationAsync(ArgumentService argumentService, ConfigurationService configService)
    {
        try
        {
            _console.ForegroundColor = ConsoleColor.Cyan;
            _console.WriteLine("\n=== Generating curl command with AI ===\n");
            _console.ResetColor();

            var aiSettings = configService.GetSettings();
            IAiClient? aiClient = null;

            if (!string.IsNullOrWhiteSpace(aiSettings.GeminiApiKey))
            {
                aiClient = new GeminiService(configService);
            }
            else if (!string.IsNullOrWhiteSpace(aiSettings.OpenAiApiKey))
            {
                aiClient = new OpenAiService(configService);
            }
            else
            {
                _console.ForegroundColor = ConsoleColor.Red;
                _console.WriteLine("Error: No AI API key configured. Use -gk or -ok to set an API key.");
                _console.ResetColor();
                return;
            }

            var curlCommand = await aiClient.GenerateCurlCommandAsync(argumentService.AiPrompt);

            if (!string.IsNullOrWhiteSpace(curlCommand))
            {
                _console.ForegroundColor = ConsoleColor.Green;
                _console.WriteLine("Generated curl command:\n");

                _console.WriteLine(curlCommand);
                _console.WriteLine();
                _console.ResetColor();

                if (argumentService.SaveCurl)
                {
                    HttpUtility.SaveToFile(curlCommand, "curl-commands", "curl", "Curl command saved to");
                }
            }
            else
            {
                _console.ForegroundColor = ConsoleColor.Red;
                _console.WriteLine("Failed to generate curl command from AI.");
                _console.ResetColor();
            }
        }
        catch (InvalidOperationException ex)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine($"Error: {ex.Message}");
            _console.ResetColor();
        }
        catch (Exception ex)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine($"Unexpected error: {ex.Message}");
            _console.ResetColor();
        }
    }

    public async Task ExecuteRequestWithPathAsync(string path, ArgumentService argumentService, ConfigurationService configService)
    {
        if (!ValidateSettingsOrExit(configService))
        {
            return;
        }

        if (argumentService.SaveCurl)
        {
            var curlService = new CurlService(configService);
            curlService.SaveCurlCommandOnly(path);
        }

        var httpClientService = new HttpClientService(configService);
        await httpClientService.ExecuteAndDisplayAsync(path, argumentService.SaveOutput);
    }

    public void GenerateCurlCommandForPath(string path, ArgumentService argumentService, ConfigurationService configService)
    {
        if (!ValidateSettingsOrExit(configService))
        {
            return;
        }

        var curlService = new CurlService(configService);
        curlService.PrintCurlCommand(path, argumentService.SaveCurl);
    }

    public async Task ExecuteDefaultRequestAsync(ArgumentService argumentService, ConfigurationService configService)
    {
        if (!ValidateSettingsOrExit(configService))
        {
            return;
        }

        if (argumentService.SaveCurl)
        {
            var curlService = new CurlService(configService);
            curlService.SaveCurlCommandOnly();
        }

        var httpClientService = new HttpClientService(configService);
        await httpClientService.ExecuteAndDisplayAsync(null, argumentService.SaveOutput);
    }

    public void GenerateDefaultCurlCommand(ArgumentService argumentService, ConfigurationService configService)
    {
        if (!ValidateSettingsOrExit(configService))
        {
            return;
        }

        var curlService = new CurlService(configService);
        curlService.PrintCurlCommand(null, argumentService.SaveCurl);
    }
}
