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

        // Validate that --output is only used with -e
        if (argumentService.SaveOutput && !argumentService.ExecuteRequest)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: -o (--output) can only be used with -e (--execute).");
            Console.ResetColor();
            return;
        }

        // Check for help flag
        if (argumentService.ShowHelp)
        {
            HelpService.DisplayHelp();
            return;
        }

        // Check for version flag
        if (argumentService.ShowVersion)
        {
            Console.WriteLine($"CoolCurl version {version}");
            return;
        }

        // Check for reset flag
        if (argumentService.ResetConfig)
        {
            argumentService.HandleResetConfig();
        }

        var configService = new ConfigurationService();

        // Check for settings display flag
        if (argumentService.ShowSettings)
        {
            configService.DisplaySettings();
            return;
        }

        // Check for method change flag
        if (argumentService.ChangeMethod)
        {
            argumentService.HandleMethodChange(configService);
            return;
        }

        // Check for URL change flag
        if (argumentService.ChangeUrl)
        {
            argumentService.HandleUrlChange(configService);
            return;
        }

        // Check for auth type change flag
        if (argumentService.ChangeAuth)
        {
            argumentService.HandleAuthChange(configService);
            return;
        }

        // Check for token change flag
        if (argumentService.ChangeToken)
        {
            argumentService.HandleTokenChange(configService);
            return;
        }

        // Check for username change flag
        if (argumentService.ChangeUsername)
        {
            argumentService.HandleUsernameChange(configService);
            return;
        }

        // Check for password change flag
        if (argumentService.ChangePassword)
        {
            argumentService.HandlePasswordChange(configService);
            return;
        }

        // Check for follow redirects change flag
        if (argumentService.ChangeFollowRedirects)
        {
            argumentService.HandleFollowRedirectsChange(configService);
            return;
        }

        // Check for show headers change flag
        if (argumentService.ChangeShowHeaders)
        {
            argumentService.HandleShowHeadersChange(configService);
            return;
        }

        // Check for show error change flag
        if (argumentService.ChangeShowError)
        {
            argumentService.HandleShowErrorChange(configService);
            return;
        }

        // Check for show progress change flag
        if (argumentService.ChangeShowProgress)
        {
            argumentService.HandleShowProgressChange(configService);
            return;
        }

        // Check for max timeout change flag
        if (argumentService.ChangeMaxTimeout)
        {
            argumentService.HandleMaxTimeoutChange(configService);
            return;
        }

        // Check for default headers change flag
        if (argumentService.ChangeDefaultHeaders)
        {
            argumentService.HandleDefaultHeadersChange(configService);
            return;
        }

        // Check for query parameters change flag
        if (argumentService.ChangeQueryParameters)
        {
            argumentService.HandleQueryParametersChange(configService);
            return;
        }

        // Check for path argument
        if (!string.IsNullOrWhiteSpace(argumentService.Path))
        {
            string? path = argumentService.Path;

            // Validate and reprompt if needed
            while (string.IsNullOrWhiteSpace(path))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Path cannot be empty.");
                Console.ResetColor();
                Console.Write("Enter path: ");
                path = Console.ReadLine();
            }

            // Add to recent paths
            configService.AddRecentPath(path);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Path '{path}' added to recent paths.");
            Console.ResetColor();

            // Generate and print curl command or execute request
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
                // Validate settings before generating curl command
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

        // If no arguments provided, generate curl command with most recent path (if available)
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

                // Save curl command if requested
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
                // Validate settings before generating curl command
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
