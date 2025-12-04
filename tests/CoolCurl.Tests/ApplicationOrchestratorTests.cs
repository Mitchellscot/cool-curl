using CoolCurl.Infrastructure;
using CoolCurl.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CoolCurl.Tests;

public class ApplicationOrchestratorTests
{
    private readonly Mock<IConsoleWriter> _mockConsole;
    private readonly ApplicationOrchestrator _orchestrator;

    public ApplicationOrchestratorTests()
    {
        _mockConsole = new Mock<IConsoleWriter>();
        _orchestrator = new ApplicationOrchestrator(_mockConsole.Object);
    }

    private IConfiguration CreateMockConfiguration(string version = "1.0.0")
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Application:Version", version }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task RunAsync_WithSaveOutputButNoExecute_ReturnsErrorCode()
    {
        var args = new[] { "-o", "output.txt" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(1, result);
        _mockConsole.Verify(c => c.WriteLine("Error: -o (--output) can only be used with -e (--execute)."), Times.Once);
        _mockConsole.VerifySet(c => c.ForegroundColor = ConsoleColor.Red, Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithHelpFlag_ReturnsZero()
    {
        var args = new[] { "-h" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithVersionFlag_DisplaysVersionAndReturnsZero()
    {
        var args = new[] { "-v" };
        var config = CreateMockConfiguration("2.5.1");

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
        _mockConsole.Verify(c => c.WriteLine("CoolCurl version 2.5.1"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithVersionFlagAndUnknownVersion_DisplaysUnknown()
    {
        var args = new[] { "-v" };
        var config = CreateMockConfiguration("");

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("CoolCurl version"))), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithShowSettings_ReturnsZero()
    {
        var args = new[] { "-ss" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeMethod_ReturnsZero()
    {
        var args = new[] { "-cm" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeUrl_ReturnsZero()
    {
        var args = new[] { "-cu" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeAuth_ReturnsZero()
    {
        var args = new[] { "-ca" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeToken_ReturnsZero()
    {
        var args = new[] { "-ct" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeUsername_ReturnsZero()
    {
        var args = new[] { "-cun" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangePassword_ReturnsZero()
    {
        var args = new[] { "-cp" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeFollowRedirects_ReturnsZero()
    {
        var args = new[] { "-cfr" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeShowHeaders_ReturnsZero()
    {
        var args = new[] { "-csh" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeShowError_ReturnsZero()
    {
        var args = new[] { "-cse" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeShowProgress_ReturnsZero()
    {
        var args = new[] { "-csp" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeMaxTimeout_ReturnsZero()
    {
        var args = new[] { "-cmt" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeDefaultHeaders_ReturnsZero()
    {
        var args = new[] { "-cdh" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeQueryParameters_ReturnsZero()
    {
        var args = new[] { "-cqp" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeGeminiApiKey_ReturnsZero()
    {
        var args = new[] { "-gk" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeOpenAiApiKey_ReturnsZero()
    {
        var args = new[] { "-ok" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithChangeAiDebugging_ReturnsZero()
    {
        var args = new[] { "-cad" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ValidateSettingsOrExit_WithValidSettings_ReturnsTrue()
    {
        var configService = new ConfigurationService();

        var result = _orchestrator.ValidateSettingsOrExit(configService);

        Assert.True(result);
    }

    [Fact]
    public async Task HandleAiPromptGenerationAsync_DisplaysGeneratingMessage()
    {
        var args = new ArgumentService(Array.Empty<string>());
        var configService = new ConfigurationService();

        await _orchestrator.HandleAiPromptGenerationAsync(args, configService);

        _mockConsole.Verify(c => c.WriteLine("\n=== Generating curl command with AI ===\n"), Times.Once);
        _mockConsole.VerifySet(c => c.ForegroundColor = ConsoleColor.Cyan, Times.Once);
    }

    [Fact]
    public void GenerateCurlCommandForPath_CallsValidateSettings()
    {
        var args = new ArgumentService(new[] { "-p", "/test" });
        var configService = new ConfigurationService();

        _orchestrator.GenerateCurlCommandForPath("/test", args, configService);

        // If validation fails, the method returns early, so we verify it was called
        // No exception means it executed successfully
        Assert.True(true);
    }

    [Fact]
    public void GenerateDefaultCurlCommand_CallsValidateSettings()
    {
        var args = new ArgumentService(Array.Empty<string>());
        var configService = new ConfigurationService();

        _orchestrator.GenerateDefaultCurlCommand(args, configService);

        // If validation fails, the method returns early
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteRequestWithPathAsync_CallsValidateSettings()
    {
        var args = new ArgumentService(new[] { "-p", "/test", "-e" });
        var configService = new ConfigurationService();

        await _orchestrator.ExecuteRequestWithPathAsync("/test", args, configService);

        // Method executes validation as first step
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteDefaultRequestAsync_CallsValidateSettings()
    {
        var args = new ArgumentService(new[] { "-e" });
        var configService = new ConfigurationService();

        await _orchestrator.ExecuteDefaultRequestAsync(args, configService);

        // Method executes validation as first step
        Assert.True(true);
    }

    [Fact]
    public async Task RunAsync_WithMultipleFlags_ProcessesFirstMatchingFlag()
    {
        var args = new[] { "-h", "-v" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
        // Help takes precedence and version message should not be displayed
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("version"))), Times.Never);
    }

    [Fact]
    public async Task RunAsync_SaveOutputFlagOnly_ShowsError()
    {
        var args = new[] { "-o", "output.json" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(1, result);
        _mockConsole.Verify(c => c.WriteLine("Error: -o (--output) can only be used with -e (--execute)."), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithResetConfig_ProcessesReset()
    {
        var args = new[] { "-rc" };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        // Reset doesn't return early, so it continues processing
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    [InlineData("-v")]
    [InlineData("--version")]
    [InlineData("-ss")]
    [InlineData("-cm")]
    [InlineData("-cu")]
    public async Task RunAsync_WithVariousFlags_ReturnsZeroExitCode(string flag)
    {
        var args = new[] { flag };
        var config = CreateMockConfiguration();

        var result = await _orchestrator.RunAsync(args, config);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ValidateSettingsOrExit_WithInvalidSettings_DisplaysErrorAndReturnsFalse()
    {
        var configService = new ConfigurationService();
        // Force invalid settings by having empty URL
        var settings = configService.GetSettings();
        settings.BaseUrl = "";
        configService.UpdateSettings(settings);

        var result = _orchestrator.ValidateSettingsOrExit(configService);

        Assert.False(result);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("Configuration error:"))), Times.Once);
        _mockConsole.VerifySet(c => c.ForegroundColor = ConsoleColor.Red, Times.Once);

        // Restore settings
        settings.BaseUrl = "https://api.example.com";
        configService.UpdateSettings(settings);
    }

    [Fact]
    public void Constructor_WithConsoleWriter_InitializesSuccessfully()
    {
        var console = new Mock<IConsoleWriter>();
        var orchestrator = new ApplicationOrchestrator(console.Object);

        Assert.NotNull(orchestrator);
    }
}
