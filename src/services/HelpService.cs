namespace CoolCurl.Services;

public class HelpService
{
    public static void DisplayHelp()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== CoolCurl Help ===\n");
        Console.ResetColor();

        Console.WriteLine("CoolCurl is a command-line HTTP client with configuration management.\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Getting Started:");
        Console.ResetColor();
        Console.WriteLine("  If this is your first time running CoolCurl, or if your configuration");
        Console.WriteLine("  file doesn't exist, we'll walk you through an interactive setup to");
        Console.WriteLine("  create your settings file.\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Options:");
        Console.ResetColor();
        Console.WriteLine("  -h, --help         Show this help message");
        Console.WriteLine("  -v, --version      Show version information");
        Console.WriteLine("  -s, --settings     Display current settings");
        Console.WriteLine("  -r, --reset        Delete configuration and start over");
        Console.WriteLine("  -p, --path <path>  Add a path to recent paths\n");

        Console.WriteLine("More features and options coming soon!\n");
    }
}
