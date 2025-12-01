namespace CoolCurl.Services;

public class ArgumentService
{
    public bool ShowHelp { get; private set; }
    public bool ShowVersion { get; private set; }
    public bool ResetConfig { get; private set; }
    public bool ShowSettings { get; private set; }
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
                case "-s":
                case "--settings":
                    ShowSettings = true;
                    break;
                case "-p":
                case "--path":
                    if (i + 1 < args.Length)
                    {
                        Path = args[i + 1];
                        i++; // Skip the next argument since we've consumed it
                    }
                    break;
                    // Add more argument cases here as needed
            }
        }
    }
}
