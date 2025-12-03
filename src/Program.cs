using CoolCurl.Services;
using Microsoft.Extensions.Configuration;

namespace CoolCurl;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var version = configuration["Application:Version"] ?? "Unknown";

        var argumentService = new ArgumentService(args);

        if (argumentService.SaveOutput && !argumentService.ExecuteRequest)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: -o (--output) can only be used with -e (--execute).");
            Console.ResetColor();
            return;
        }

        if (argumentService.ShowHelp)
        {
            HelpService.DisplayHelp();
            return;
        }

        if (argumentService.ShowVersion)
        {
            Console.WriteLine($"CoolCurl version {version}");
            return;
        }

        if (argumentService.ResetConfig)
        {
            argumentService.HandleResetConfig();
        }

        var configService = new ConfigurationService();

        if (argumentService.ShowSettings)
        {
            configService.DisplaySettings();
            return;
        }

        if (argumentService.ChangeMethod)
        {
            argumentService.HandleMethodChange(configService);
            return;
        }

        if (argumentService.ChangeUrl)
        {
            argumentService.HandleUrlChange(configService);
            return;
        }

        if (argumentService.ChangeAuth)
        {
            argumentService.HandleAuthChange(configService);
            return;
        }

        if (argumentService.ChangeToken)
        {
            argumentService.HandleTokenChange(configService);
            return;
        }

        if (argumentService.ChangeUsername)
        {
            argumentService.HandleUsernameChange(configService);
            return;
        }

        if (argumentService.ChangePassword)
        {
            argumentService.HandlePasswordChange(configService);
            return;
        }

        if (argumentService.ChangeFollowRedirects)
        {
            argumentService.HandleFollowRedirectsChange(configService);
            return;
        }

        if (argumentService.ChangeShowHeaders)
        {
            argumentService.HandleShowHeadersChange(configService);
            return;
        }

        if (argumentService.ChangeShowError)
        {
            argumentService.HandleShowErrorChange(configService);
            return;
        }

        if (argumentService.ChangeShowProgress)
        {
            argumentService.HandleShowProgressChange(configService);
            return;
        }

        if (argumentService.ChangeMaxTimeout)
        {
            argumentService.HandleMaxTimeoutChange(configService);
            return;
        }

        if (argumentService.ChangeDefaultHeaders)
        {
            argumentService.HandleDefaultHeadersChange(configService);
            return;
        }

        if (argumentService.ChangeQueryParameters)
        {
            argumentService.HandleQueryParametersChange(configService);
            return;
        }

        if (argumentService.ChangeGeminiApiKey)
        {
            argumentService.HandleGeminiApiKeyChange(configService);
            return;
        }

        if (argumentService.ChangeAiDebugging)
        {
            argumentService.HandleAiDebuggingChange(configService);
            return;
        }

        if (argumentService.UseAiPrompt && !string.IsNullOrWhiteSpace(argumentService.AiPrompt))
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=== Generating curl command with AI ===\n");
                Console.ResetColor();

                var geminiService = new GeminiService(configService);
                var curlCommand = await geminiService.GenerateCurlCommandAsync(argumentService.AiPrompt);

                if (!string.IsNullOrWhiteSpace(curlCommand))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Generated curl command:\n");
                    Console.ResetColor();
                    Console.WriteLine(curlCommand);
                    Console.WriteLine();

                    if (argumentService.SaveCurl)
                    {
                        HttpUtility.SaveToFile(curlCommand, "curl-commands", "curl", "Curl command saved to");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to generate curl command from AI.");
                    Console.ResetColor();
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.ResetColor();
            }
            return;
        }

        if (!string.IsNullOrWhiteSpace(argumentService.Path))
        {
            string? path = argumentService.Path;

            while (string.IsNullOrWhiteSpace(path))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Path cannot be empty.");
                Console.ResetColor();
                Console.Write("Enter path: ");
                path = Console.ReadLine();
            }

            configService.AddRecentPath(path);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Path '{path}' added to recent paths.");
            Console.ResetColor();

            if (argumentService.ExecuteRequest)
            {
                // Validate settings before executing
                if (!configService.ValidateSettings(out string errorMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Configuration error: {errorMessage}");
                    Console.ResetColor();
                    return;
                }

                // Save curl command if requested
                if (argumentService.SaveCurl)
                {
                    var curlService = new CurlService(configService);
                    curlService.SaveCurlCommandOnly(path);
                }

                var httpClientService = new HttpClientService(configService);
                await httpClientService.ExecuteAndDisplayAsync(path, argumentService.SaveOutput);
            }
            else
            {
                if (!configService.ValidateSettings(out string errorMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Configuration error: {errorMessage}");
                    Console.ResetColor();
                    return;
                }

                var curlService = new CurlService(configService);
                curlService.PrintCurlCommand(path, argumentService.SaveCurl);
            }
            return;
        }

        var settings = configService.GetSettings();
        if (settings.RecentPaths.Count > 0)
        {
            if (argumentService.ExecuteRequest)
            {
                // Validate settings before executing
                if (!configService.ValidateSettings(out string errorMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Configuration error: {errorMessage}");
                    Console.ResetColor();
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
            else
            {
                if (!configService.ValidateSettings(out string errorMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Configuration error: {errorMessage}");
                    Console.ResetColor();
                    return;
                }

                var defaultCurlService = new CurlService(configService);
                defaultCurlService.PrintCurlCommand(null, argumentService.SaveCurl);
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No recent URL paths available. Use -p <path> to add a path.");
            Console.ResetColor();
        }
    }
}
