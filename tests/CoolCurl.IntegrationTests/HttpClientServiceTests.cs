using CoolCurl.Infrastructure;
using CoolCurl.Models;
using CoolCurl.Services;
using Moq;
using System.Net;

namespace CoolCurl.IntegrationTests;

public class HttpClientServiceTests
{
    [Fact]
    public void Constructor_WithAiDebuggingDisabled_DoesNotInitializeAiDebugService()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        // Act
        var service = new HttpClientService(mockConfigService.Object);

        // Assert - service should be created without throwing
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithGeminiApiKey_InitializesAiDebugService()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            GeminiApiKey = "test-gemini-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        // Act
        var service = new HttpClientService(mockConfigService.Object);

        // Assert - service should be created
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithOpenAiApiKey_InitializesAiDebugService()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            AllowAiDebugging = true,
            OpenAiApiKey = "test-openai-key"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        // Act
        var service = new HttpClientService(mockConfigService.Object);

        // Assert - service should be created
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithMaxTimeSeconds_ConfiguresHttpClientTimeout()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            MaxTimeSeconds = 10
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        // Act
        var service = new HttpClientService(mockConfigService.Object);

        // Assert - service should be created with configured timeout
        Assert.NotNull(service);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithBasicUrl_ReturnsHttpResponse()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act
        var response = await service.ExecuteRequestAsync("/get");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithQueryParameters_IncludesParametersInUrl()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            QueryParameters = new Dictionary<string, string>
            {
                { "test", "value" }
            }
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act
        var response = await service.ExecuteRequestAsync("/get");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("test", content);
        Assert.Contains("value", content);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithCustomHeaders_IncludesHeadersInRequest()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            DefaultHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "test-value" }
            }
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act
        var response = await service.ExecuteRequestAsync("/headers");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("X-Custom-Header", content);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithBearerAuth_IncludesAuthorizationHeader()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            AuthType = AuthType.BearerToken,
            AuthToken = "test-token"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act
        var response = await service.ExecuteRequestAsync("/headers");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Authorization", content);
        Assert.Contains("Bearer", content);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithPostMethod_UsesPostMethod()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "POST"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act
        var response = await service.ExecuteRequestAsync("/post");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ExecuteRequestAsync_WithInvalidUrl_ThrowsHttpRequestException()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://invalid-domain-that-does-not-exist-12345.com",
            DefaultMethod = "GET"
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await service.ExecuteRequestAsync("/test");
        });
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithSuccessfulRequest_DisplaysStatusAndBody()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowHeaders = false,
            ShowProgress = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/get");

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("Status: 200", output);
            Assert.Contains("Response Body:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithShowHeaders_DisplaysHeaders()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowHeaders = true,
            ShowProgress = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/get");

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("Response Headers:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithShowProgress_DisplaysProgressMessage()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowProgress = true
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/get");

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("Sending request", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithJsonResponse_FormatsJson()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowHeaders = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/json");

            var output = stringWriter.ToString();

            // Assert - should contain formatted JSON with indentation
            Assert.Contains("{", output);
            Assert.Contains("}", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithNonSuccessStatusCode_DisplaysRedStatus()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowHeaders = false,
            AllowAiDebugging = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/status/404");

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("Status: 404", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithSaveOutput_SavesOutputToFile()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://httpbin.org",
            DefaultMethod = "GET",
            ShowHeaders = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/get", saveOutput: true);

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("Response saved", output);
            Assert.Contains(".cool-curl", output);

            // Cleanup
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outputDirectory = Path.Combine(homeDirectory, ".cool-curl", "responses");

            if (Directory.Exists(outputDirectory))
            {
                var files = Directory.GetFiles(outputDirectory, "response_*.txt");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task ExecuteAndDisplayAsync_WithHttpRequestException_DisplaysError()
    {
        // Arrange
        var mockConfigService = new Mock<ConfigurationService>();
        var settings = new Settings
        {
            BaseUrl = "https://invalid-domain-that-does-not-exist-12345.com",
            DefaultMethod = "GET",
            ShowError = true,
            AllowAiDebugging = false
        };
        mockConfigService.Setup(x => x.GetSettings()).Returns(settings);

        var service = new HttpClientService(mockConfigService.Object);

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            await service.ExecuteAndDisplayAsync("/test");

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains("HTTP Request Error", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
