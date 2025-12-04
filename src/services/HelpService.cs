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
        Console.WriteLine("  -h,  --help              Show this help message");
        Console.WriteLine("  -v,  --version           Show version information");
        Console.WriteLine("  -c,  --config            Display current settings");
        Console.WriteLine("  -r,  --reset             Delete configuration and start over");
        Console.WriteLine("  -p,  --path <path>       Add a path to recent paths");
        Console.WriteLine("  -e,  --execute           Execute the HTTP request");
        Console.WriteLine("  -s,  --save              Save the curl command to disk");
        Console.WriteLine("  -o,  --output            Save the HTTP response to disk (requires -e)\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Configuration Change Options:");
        Console.ResetColor();
        Console.WriteLine("  -m,  --method            Change the default HTTP method");
        Console.WriteLine("  -u,  --url               Change the base URL");
        Console.WriteLine("  -a,  --auth              Change the authentication type");
        Console.WriteLine("  -t,  --token             Change the auth token");
        Console.WriteLine("  -un, --username          Change the basic auth username");
        Console.WriteLine("  -pw, --password          Change the basic auth password");
        Console.WriteLine("  -f,  --follow            Change follow redirects setting");
        Console.WriteLine("  -sh, --show-headers      Change show headers setting");
        Console.WriteLine("  -se, --show-errors       Change show errors setting");
        Console.WriteLine("  -sp, --show-progress     Change show progress setting");
        Console.WriteLine("  -mt, --max-timeout       Change max timeout in seconds");
        Console.WriteLine("  -dh, --default-headers   Manage default headers");
        Console.WriteLine("  -qp, --query-parameters  Manage query parameters");
        Console.WriteLine("  -gk, --gemini-key        Change the Google Gemini API key");
        Console.WriteLine("  -ok, --openai-key        Change the OpenAI API key");
        Console.WriteLine("  -ad, --ai-debugging      Enable/disable AI-powered error debugging\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("AI Features:");
        Console.ResetColor();
        Console.WriteLine("  -ai, --ai-prompt <prompt>  Generate curl command using AI prompt");
        Console.WriteLine("                             Example: -ai \"make a GET request to /users\"");
        Console.WriteLine("                             Supports Google Gemini or OpenAI (gpt-4o-mini)");
        Console.WriteLine("  AI Debugging:              When enabled, HTTP errors are analyzed by AI");
        Console.WriteLine("                             to suggest fixes and troubleshooting steps");

        Console.WriteLine("More features and options coming soon!\n");
    }
}
