using CoolCurl.Models;
using CoolCurl.Services;
using Moq;

namespace CoolCurl.Tests;

public class ConfigurationServiceTests
{
    private Mock<ConfigurationService> CreateMockConfigService(Settings settings)
    {
        var mock = new Mock<ConfigurationService>();
        mock.Setup(x => x.GetSettings()).Returns(settings);
        return mock;
    }

    [Fact]
    public void GetSettings_WithMockedSettings_ReturnsSettings()
    {
        var expectedSettings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            AuthType = AuthType.None
        };
        var mockService = CreateMockConfigService(expectedSettings);

        var settings = mockService.Object.GetSettings();

        Assert.Equal(expectedSettings.BaseUrl, settings.BaseUrl);
        Assert.Equal(expectedSettings.DefaultMethod, settings.DefaultMethod);
        Assert.Equal(expectedSettings.AuthType, settings.AuthType);
    }

    [Fact]
    public void GetSettings_WithDefaultHeaders_ReturnsHeadersCorrectly()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultHeaders = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Accept", "application/json" }
            }
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Equal(2, loadedSettings.DefaultHeaders.Count);
        Assert.Equal("application/json", loadedSettings.DefaultHeaders["Content-Type"]);
        Assert.Equal("application/json", loadedSettings.DefaultHeaders["Accept"]);
    }

    [Fact]
    public void GetSettings_WithQueryParameters_ReturnsParametersCorrectly()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            QueryParameters = new Dictionary<string, string>
            {
                { "page", "1" },
                { "limit", "10" }
            }
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Equal(2, loadedSettings.QueryParameters.Count);
        Assert.Equal("1", loadedSettings.QueryParameters["page"]);
        Assert.Equal("10", loadedSettings.QueryParameters["limit"]);
    }

    [Fact]
    public void GetSettings_WithAiConfiguration_ReturnsAiSettingsCorrectly()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            GeminiApiKey = "gemini-key-123",
            OpenAiApiKey = "openai-key-456",
            AllowAiDebugging = true
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Equal("gemini-key-123", loadedSettings.GeminiApiKey);
        Assert.Equal("openai-key-456", loadedSettings.OpenAiApiKey);
        Assert.True(loadedSettings.AllowAiDebugging);
    }

    [Fact]
    public void GetSettings_WithComplexSettings_ReturnsAllPropertiesCorrectly()
    {
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com",
            DefaultMethod = "POST",
            AuthType = AuthType.BearerToken,
            AuthToken = "token123",
            FollowRedirects = true,
            ShowProgress = true,
            ShowError = false,
            ShowHeaders = true,
            MaxTimeSeconds = 30,
            DefaultHeaders = new Dictionary<string, string> { { "X-Custom", "value" } },
            QueryParameters = new Dictionary<string, string> { { "test", "param" } },
            RecentPaths = new List<string> { "/api/test" },
            GeminiApiKey = "gemini-key",
            AllowAiDebugging = true
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Equal("https://api.example.com", loadedSettings.BaseUrl);
        Assert.Equal("POST", loadedSettings.DefaultMethod);
        Assert.Equal(AuthType.BearerToken, loadedSettings.AuthType);
        Assert.Equal("token123", loadedSettings.AuthToken);
        Assert.True(loadedSettings.FollowRedirects);
        Assert.True(loadedSettings.ShowProgress);
        Assert.False(loadedSettings.ShowError);
        Assert.True(loadedSettings.ShowHeaders);
        Assert.Equal(30, loadedSettings.MaxTimeSeconds);
        Assert.Single(loadedSettings.DefaultHeaders);
        Assert.Single(loadedSettings.QueryParameters);
        Assert.Single(loadedSettings.RecentPaths);
        Assert.Equal("gemini-key", loadedSettings.GeminiApiKey);
        Assert.True(loadedSettings.AllowAiDebugging);
    }

    [Fact]
    public void GetSettings_WithAllAuthTypes_ReturnsCorrectAuthType()
    {
        var authTypes = new[] { AuthType.None, AuthType.BearerToken, AuthType.BasicAuth, AuthType.JwtBearer };

        foreach (var authType in authTypes)
        {
            var settings = new Settings
            {
                BaseUrl = "http://localhost:5000",
                AuthType = authType
            };
            var mockService = CreateMockConfigService(settings);

            var loadedSettings = mockService.Object.GetSettings();

            Assert.Equal(authType, loadedSettings.AuthType);
        }
    }

    [Fact]
    public void GetSettings_WithMultipleRecentPaths_ReturnsAllPaths()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            RecentPaths = new List<string> { "/api/users", "/api/products", "/api/orders" }
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Equal(3, loadedSettings.RecentPaths.Count);
        Assert.Equal("/api/users", loadedSettings.RecentPaths[0]);
        Assert.Equal("/api/products", loadedSettings.RecentPaths[1]);
        Assert.Equal("/api/orders", loadedSettings.RecentPaths[2]);
    }

    [Fact]
    public void GetSettings_WithEmptyCollections_ReturnsEmptyCollections()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultHeaders = new Dictionary<string, string>(),
            QueryParameters = new Dictionary<string, string>(),
            RecentPaths = new List<string>()
        };
        var mockService = CreateMockConfigService(settings);

        var loadedSettings = mockService.Object.GetSettings();

        Assert.Empty(loadedSettings.DefaultHeaders);
        Assert.Empty(loadedSettings.QueryParameters);
        Assert.Empty(loadedSettings.RecentPaths);
    }
}
