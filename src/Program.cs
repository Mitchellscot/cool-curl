using CoolCurl.Services;
using Microsoft.Extensions.Configuration;

namespace CoolCurl;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var version = configuration["Application:Version"] ?? "Unknown";

        var argumentService = new ArgumentService(args);

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
            var configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cool_curl_config"
            );

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Configuration deleted successfully.");
                Console.ResetColor();
            }
        }

        var configService = new ConfigurationService();

        // Check for settings display flag
        if (argumentService.ShowSettings)
        {
            configService.DisplaySettings();
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

            // Generate and print curl command
            var curlService = new CurlService(configService);
            curlService.PrintCurlCommand(path);
            return;
        }

        // If no arguments provided, generate curl command with most recent path (if available)
        var settings = configService.GetSettings();
        if (settings.RecentPaths.Count > 0)
        {
            var defaultCurlService = new CurlService(configService);
            defaultCurlService.PrintCurlCommand();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No recent paths available. Use -p <path> to add a path.");
            Console.ResetColor();
        }
    }
}
