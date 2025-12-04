using CoolCurl.Infrastructure;
using CoolCurl.Models;
using CoolCurl.Services;
using Moq;

namespace CoolCurl.IntegrationTests;

public class AiDebugServiceTests
{
    [Fact]
    public async Task DebugHttpErrorAsync_WithAiDebuggingDisabled_ReturnsNull()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings { AllowAiDebugging = false };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        Assert.Null(result);
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WithNoApiKey_ReturnsNull()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = null,
            GeminiApiKey = null
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        Assert.Null(result);
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WithValidConfiguration_CallsAiClient()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        // Act
        var result = await service.DebugHttpErrorAsync(exception, "https://api.example.com", "GET");

        // Assert
        Assert.Equal("Debug suggestion", result);
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(
            It.Is<string>(s => s.Contains("Test error")),
            It.Is<string>(s => s.Contains("https://api.example.com") && s.Contains("GET"))
        ), Times.Once);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WithHttpRequestException_IncludesStatusCode()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            GeminiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Not Found", null, System.Net.HttpStatusCode.NotFound);

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(
            It.Is<string>(s => s.Contains("404") && s.Contains("NotFound")),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WithTaskCanceledException_IndicatesTimeout()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new TaskCanceledException("Request timeout");

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(
            It.Is<string>(s => s.Contains("Timeout")),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WithInnerException_IncludesInnerExceptionMessage()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var innerException = new Exception("Inner error message");
        var exception = new HttpRequestException("Outer error", innerException);

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(
            It.Is<string>(s => s.Contains("Inner error message")),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_IncludesRequestDetails()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key",
            AuthType = AuthType.BearerToken,
            MaxTimeSeconds = 30,
            DefaultHeaders = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        // Act
        var result = await service.DebugHttpErrorAsync(exception, "https://api.example.com/users", "POST");

        // Assert
        mockAiClient.Verify(x => x.DebugHttpErrorAsync(
            It.IsAny<string>(),
            It.Is<string>(s =>
                s.Contains("https://api.example.com/users") &&
                s.Contains("POST") &&
                s.Contains("BearerToken") &&
                s.Contains("30 seconds") &&
                s.Contains("Content-Type"))
        ), Times.Once);
    }

    [Fact]
    public async Task DebugHttpErrorAsync_WhenAiClientThrows_ReturnsNull()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("AI service error"));

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        // Act
        var result = await service.DebugHttpErrorAsync(exception);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DisplayDebugSuggestionsAsync_WithValidSuggestions_DisplaysToConsole()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);
        mockAiClient.Setup(x => x.DebugHttpErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Debug suggestion");

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.DisplayDebugSuggestionsAsync(exception);

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("AI Debug Suggestions", output);
            Assert.Contains("Debug suggestion", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task DisplayDebugSuggestionsAsync_WithNoSuggestions_DoesNotDisplay()
    {
        // Arrange
        var mockAiClient = new Mock<IAiClient>();
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new AiDebugService(mockAiClient.Object, mockConfigService.Object);
        var exception = new HttpRequestException("Test error");

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.DisplayDebugSuggestionsAsync(exception);

            var output = stringWriter.ToString();

            // Assert
            Assert.DoesNotContain("AI Debug Suggestions", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
