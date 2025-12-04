using CoolCurl.Models;

namespace CoolCurl.Tests;

public class SettingsTests
{
    [Fact]
    public void Settings_CanBeInstantiated()
    {
        var settings = new Settings();

        Assert.NotNull(settings);
    }

    [Fact]
    public void Settings_HasDefaultRecentPaths()
    {
        var settings = new Settings();

        Assert.NotNull(settings.RecentPaths);
        Assert.Empty(settings.RecentPaths);
    }

    [Fact]
    public void Settings_HasDefaultAuthType()
    {
        var settings = new Settings();

        Assert.Equal(AuthType.None, settings.AuthType);
    }

    [Fact]
    public void Settings_HasDefaultFollowRedirects()
    {
        var settings = new Settings();

        Assert.True(settings.FollowRedirects);
    }

    [Fact]
    public void Settings_HasDefaultShowProgress()
    {
        var settings = new Settings();

        Assert.False(settings.ShowProgress);
    }

    [Fact]
    public void Settings_HasDefaultShowError()
    {
        var settings = new Settings();

        Assert.True(settings.ShowError);
    }

    [Fact]
    public void Settings_HasDefaultShowHeaders()
    {
        var settings = new Settings();

        Assert.True(settings.ShowHeaders);
    }

    [Fact]
    public void Settings_HasDefaultHeaders()
    {
        var settings = new Settings();

        Assert.NotNull(settings.DefaultHeaders);
        Assert.Equal(3, settings.DefaultHeaders.Count);
        Assert.Equal("application/json", settings.DefaultHeaders["Accept"]);
        Assert.Equal("application/json", settings.DefaultHeaders["Content-Type"]);
        Assert.Equal("CoolCurl/1.0", settings.DefaultHeaders["User-Agent"]);
    }

    [Fact]
    public void Settings_HasDefaultQueryParameters()
    {
        var settings = new Settings();

        Assert.NotNull(settings.QueryParameters);
        Assert.Empty(settings.QueryParameters);
    }

    [Fact]
    public void Settings_HasDefaultAllowAiDebugging()
    {
        var settings = new Settings();

        Assert.False(settings.AllowAiDebugging);
    }

    [Fact]
    public void Settings_CanSetBaseUrl()
    {
        var settings = new Settings { BaseUrl = "https://api.example.com" };

        Assert.Equal("https://api.example.com", settings.BaseUrl);
    }

    [Fact]
    public void Settings_CanSetDefaultMethod()
    {
        var settings = new Settings { DefaultMethod = "POST" };

        Assert.Equal("POST", settings.DefaultMethod);
    }

    [Fact]
    public void Settings_CanAddRecentPaths()
    {
        var settings = new Settings();
        settings.RecentPaths.Add("/api/users");
        settings.RecentPaths.Add("/api/products");

        Assert.Equal(2, settings.RecentPaths.Count);
        Assert.Contains("/api/users", settings.RecentPaths);
        Assert.Contains("/api/products", settings.RecentPaths);
    }

    [Fact]
    public void Settings_CanSetAuthType()
    {
        var settings = new Settings { AuthType = AuthType.BearerToken };

        Assert.Equal(AuthType.BearerToken, settings.AuthType);
    }

    [Fact]
    public void Settings_CanSetAuthToken()
    {
        var settings = new Settings { AuthToken = "token123" };

        Assert.Equal("token123", settings.AuthToken);
    }

    [Fact]
    public void Settings_CanSetBasicAuthUsername()
    {
        var settings = new Settings { BasicAuthUsername = "testuser" };

        Assert.Equal("testuser", settings.BasicAuthUsername);
    }

    [Fact]
    public void Settings_CanSetBasicAuthPassword()
    {
        var settings = new Settings { BasicAuthPassword = "testpass" };

        Assert.Equal("testpass", settings.BasicAuthPassword);
    }

    [Fact]
    public void Settings_CanSetFollowRedirects()
    {
        var settings = new Settings { FollowRedirects = false };

        Assert.False(settings.FollowRedirects);
    }

    [Fact]
    public void Settings_CanSetShowProgress()
    {
        var settings = new Settings { ShowProgress = true };

        Assert.True(settings.ShowProgress);
    }

    [Fact]
    public void Settings_CanSetShowError()
    {
        var settings = new Settings { ShowError = false };

        Assert.False(settings.ShowError);
    }

    [Fact]
    public void Settings_CanSetShowHeaders()
    {
        var settings = new Settings { ShowHeaders = false };

        Assert.False(settings.ShowHeaders);
    }

    [Fact]
    public void Settings_CanSetMaxTimeSeconds()
    {
        var settings = new Settings { MaxTimeSeconds = 60 };

        Assert.Equal(60, settings.MaxTimeSeconds);
    }

    [Fact]
    public void Settings_CanAddDefaultHeaders()
    {
        var settings = new Settings();
        settings.DefaultHeaders["X-Custom-Header"] = "custom-value";

        Assert.Equal(4, settings.DefaultHeaders.Count);
        Assert.Equal("custom-value", settings.DefaultHeaders["X-Custom-Header"]);
    }

    [Fact]
    public void Settings_CanModifyDefaultHeaders()
    {
        var settings = new Settings();
        settings.DefaultHeaders["Accept"] = "text/html";

        Assert.Equal("text/html", settings.DefaultHeaders["Accept"]);
    }

    [Fact]
    public void Settings_CanClearDefaultHeaders()
    {
        var settings = new Settings();
        settings.DefaultHeaders.Clear();

        Assert.Empty(settings.DefaultHeaders);
    }

    [Fact]
    public void Settings_CanAddQueryParameters()
    {
        var settings = new Settings();
        settings.QueryParameters["page"] = "1";
        settings.QueryParameters["limit"] = "10";

        Assert.Equal(2, settings.QueryParameters.Count);
        Assert.Equal("1", settings.QueryParameters["page"]);
        Assert.Equal("10", settings.QueryParameters["limit"]);
    }

    [Fact]
    public void Settings_CanSetGeminiApiKey()
    {
        var settings = new Settings { GeminiApiKey = "gemini-key-123" };

        Assert.Equal("gemini-key-123", settings.GeminiApiKey);
    }

    [Fact]
    public void Settings_CanSetOpenAiApiKey()
    {
        var settings = new Settings { OpenAiApiKey = "openai-key-123" };

        Assert.Equal("openai-key-123", settings.OpenAiApiKey);
    }

    [Fact]
    public void Settings_CanSetAllowAiDebugging()
    {
        var settings = new Settings { AllowAiDebugging = true };

        Assert.True(settings.AllowAiDebugging);
    }

    [Fact]
    public void Settings_NullMaxTimeSeconds_IsAllowed()
    {
        var settings = new Settings { MaxTimeSeconds = null };

        Assert.Null(settings.MaxTimeSeconds);
    }

    [Fact]
    public void Settings_NullBaseUrl_IsAllowed()
    {
        var settings = new Settings { BaseUrl = null };

        Assert.Null(settings.BaseUrl);
    }

    [Fact]
    public void Settings_NullDefaultMethod_IsAllowed()
    {
        var settings = new Settings { DefaultMethod = null };

        Assert.Null(settings.DefaultMethod);
    }

    [Fact]
    public void Settings_WithAllAuthTypes_Works()
    {
        var settings1 = new Settings { AuthType = AuthType.None };
        var settings2 = new Settings { AuthType = AuthType.BearerToken };
        var settings3 = new Settings { AuthType = AuthType.BasicAuth };
        var settings4 = new Settings { AuthType = AuthType.JwtBearer };

        Assert.Equal(AuthType.None, settings1.AuthType);
        Assert.Equal(AuthType.BearerToken, settings2.AuthType);
        Assert.Equal(AuthType.BasicAuth, settings3.AuthType);
        Assert.Equal(AuthType.JwtBearer, settings4.AuthType);
    }

    [Fact]
    public void Settings_CanRemoveRecentPath()
    {
        var settings = new Settings();
        settings.RecentPaths.Add("/api/test");
        settings.RecentPaths.Remove("/api/test");

        Assert.Empty(settings.RecentPaths);
    }

    [Fact]
    public void Settings_CanRemoveQueryParameter()
    {
        var settings = new Settings();
        settings.QueryParameters["key"] = "value";
        settings.QueryParameters.Remove("key");

        Assert.Empty(settings.QueryParameters);
    }

    [Fact]
    public void Settings_CanRemoveDefaultHeader()
    {
        var settings = new Settings();
        settings.DefaultHeaders.Remove("Accept");

        Assert.Equal(2, settings.DefaultHeaders.Count);
        Assert.False(settings.DefaultHeaders.ContainsKey("Accept"));
    }
}
