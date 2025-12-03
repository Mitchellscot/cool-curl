using CoolCurl.Models;
using CoolCurl.Services;
using Moq;

namespace CoolCurl.Tests;

public class CurlServiceTests
{
    private Mock<ConfigurationService> CreateMockConfigService(Settings settings)
    {
        var mock = new Mock<ConfigurationService>();
        mock.Setup(x => x.GetSettings()).Returns(settings);
        return mock;
    }

    [Fact]
    public void BuildCurlCommand_WithBasicSettings_ReturnsSimpleGetCommand()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            DefaultHeaders = new Dictionary<string, string>(),
            FollowRedirects = false,
            ShowError = false,
            ShowHeaders = false
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Equal("curl \"http://localhost:5000/api/users\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithPostMethod_IncludesMethodFlag()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "POST",
            RecentPaths = new List<string> { "/api/users" },
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-X POST", result);
    }

    [Fact]
    public void BuildCurlCommand_WithPathOverride_UsesOverridePath()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand("/api/products");

        Assert.Contains("http://localhost:5000/api/products", result);
    }

    [Fact]
    public void BuildCurlCommand_WithBearerToken_IncludesAuthorizationHeader()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            AuthType = AuthType.BearerToken,
            AuthToken = "test-token-123",
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-H \"Authorization: Bearer test-token-123\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithBasicAuth_IncludesCredentials()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            AuthType = AuthType.BasicAuth,
            BasicAuthUsername = "admin",
            BasicAuthPassword = "password123",
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-u \"admin:password123\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithBasicAuthUsernameOnly_IncludesUsername()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            AuthType = AuthType.BasicAuth,
            BasicAuthUsername = "admin",
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-u \"admin\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithJwtBearer_IncludesAuthorizationHeader()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            AuthType = AuthType.JwtBearer,
            AuthToken = "jwt-token-456",
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-H \"Authorization: Bearer jwt-token-456\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithFollowRedirects_IncludesLFlag()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            FollowRedirects = true,
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-L", result);
    }

    [Fact]
    public void BuildCurlCommand_WithShowProgress_IncludesProgressBar()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            ShowProgress = true,
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("--progress-bar", result);
    }

    [Fact]
    public void BuildCurlCommand_WithShowError_IncludesShowErrorFlag()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            ShowError = true,
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("--show-error", result);
    }

    [Fact]
    public void BuildCurlCommand_WithShowHeaders_IncludesIFlag()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            ShowHeaders = true,
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-i", result);
    }

    [Fact]
    public void BuildCurlCommand_WithMaxTimeout_IncludesMaxTimeFlag()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            MaxTimeSeconds = 30,
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("--max-time 30", result);
    }

    [Fact]
    public void BuildCurlCommand_WithDefaultHeaders_IncludesHeaders()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            DefaultHeaders = new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "Content-Type", "application/json" }
            }
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-H \"Accept: application/json\"", result);
        Assert.Contains("-H \"Content-Type: application/json\"", result);
    }

    [Fact]
    public void BuildCurlCommand_WithQueryParameters_IncludesQueryString()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "GET",
            RecentPaths = new List<string> { "/api/users" },
            QueryParameters = new Dictionary<string, string>
            {
                { "page", "1" },
                { "limit", "10" }
            },
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("page=1", result);
        Assert.Contains("limit=10", result);
    }

    [Fact]
    public void BuildCurlCommand_WithAllFeatures_CombinesAllOptions()
    {
        var settings = new Settings
        {
            BaseUrl = "http://localhost:5000",
            DefaultMethod = "POST",
            RecentPaths = new List<string> { "/api/users" },
            AuthType = AuthType.BearerToken,
            AuthToken = "token123",
            FollowRedirects = true,
            ShowProgress = true,
            ShowError = true,
            ShowHeaders = true,
            MaxTimeSeconds = 60,
            DefaultHeaders = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            },
            QueryParameters = new Dictionary<string, string>
            {
                { "test", "value" }
            }
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);

        var result = service.BuildCurlCommand();

        Assert.Contains("-X POST", result);
        Assert.Contains("-H \"Authorization: Bearer token123\"", result);
        Assert.Contains("-L", result);
        Assert.Contains("--progress-bar", result);
        Assert.Contains("--show-error", result);
        Assert.Contains("-i", result);
        Assert.Contains("--max-time 60", result);
        Assert.Contains("-H \"Accept: application/json\"", result);
        Assert.Contains("test=value", result);
    }

    [Fact]
    public void AddAuthentication_WithNoAuth_AddsNothing()
    {
        var settings = new Settings
        {
            AuthType = AuthType.None
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);
        var curl = new System.Text.StringBuilder("curl");

        service.AddAuthentication(curl, settings);

        Assert.Equal("curl", curl.ToString());
    }

    [Fact]
    public void AddHeaders_WithMultipleHeaders_AddsAllHeaders()
    {
        var settings = new Settings
        {
            DefaultHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "value1" },
                { "X-Another-Header", "value2" }
            }
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);
        var curl = new System.Text.StringBuilder("curl");

        service.AddHeaders(curl, settings);

        var result = curl.ToString();
        Assert.Contains("-H \"X-Custom-Header: value1\"", result);
        Assert.Contains("-H \"X-Another-Header: value2\"", result);
    }

    [Fact]
    public void AddHeaders_WithEmptyDictionary_AddsNothing()
    {
        var settings = new Settings
        {
            DefaultHeaders = new Dictionary<string, string>()
        };
        var mockConfig = CreateMockConfigService(settings);
        var service = new CurlService(mockConfig.Object);
        var curl = new System.Text.StringBuilder("curl");

        service.AddHeaders(curl, settings);

        Assert.Equal("curl", curl.ToString());
    }
}
