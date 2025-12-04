using CoolCurl.Services;
using Microsoft.Extensions.Configuration;

namespace CoolCurl;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var console = new ConsoleWriter();
        var orchestrator = new ApplicationOrchestrator(console);

        return await orchestrator.RunAsync(args, configuration);
    }
}
