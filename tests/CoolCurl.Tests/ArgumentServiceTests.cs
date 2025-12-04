using CoolCurl.Services;

namespace CoolCurl.Tests;

public class ArgumentServiceTests
{
    [Fact]
    public void Constructor_WithNoArguments_SetsAllFlagsToFalse()
    {
        var args = Array.Empty<string>();

        var service = new ArgumentService(args);

        Assert.False(service.ShowHelp);
        Assert.False(service.ShowVersion);
        Assert.False(service.ResetConfig);
        Assert.False(service.ShowSettings);
        Assert.False(service.ExecuteRequest);
        Assert.False(service.SaveCurl);
        Assert.False(service.SaveOutput);
        Assert.False(service.UseAiPrompt);
        Assert.Null(service.Path);
        Assert.Null(service.AiPrompt);
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    public void ParseArguments_WithHelpFlag_SetsShowHelpToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ShowHelp);
    }

    [Theory]
    [InlineData("-v")]
    [InlineData("--version")]
    public void ParseArguments_WithVersionFlag_SetsShowVersionToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ShowVersion);
    }

    [Theory]
    [InlineData("-r")]
    [InlineData("--reset")]
    public void ParseArguments_WithResetFlag_SetsResetConfigToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ResetConfig);
    }

    [Theory]
    [InlineData("-c")]
    [InlineData("--config")]
    public void ParseArguments_WithConfigFlag_SetsShowSettingsToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ShowSettings);
    }

    [Theory]
    [InlineData("-e")]
    [InlineData("--execute")]
    public void ParseArguments_WithExecuteFlag_SetsExecuteRequestToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ExecuteRequest);
    }

    [Theory]
    [InlineData("-s")]
    [InlineData("--save")]
    public void ParseArguments_WithSaveFlag_SetsSaveCurlToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.SaveCurl);
    }

    [Theory]
    [InlineData("-o")]
    [InlineData("--output")]
    public void ParseArguments_WithOutputFlag_SetsSaveOutputToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.SaveOutput);
    }

    [Theory]
    [InlineData("-m")]
    [InlineData("--method")]
    public void ParseArguments_WithMethodFlag_SetsChangeMethodToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeMethod);
    }

    [Theory]
    [InlineData("-u")]
    [InlineData("--url")]
    public void ParseArguments_WithUrlFlag_SetsChangeUrlToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeUrl);
    }

    [Theory]
    [InlineData("-a")]
    [InlineData("--auth")]
    public void ParseArguments_WithAuthFlag_SetsChangeAuthToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeAuth);
    }

    [Theory]
    [InlineData("-t")]
    [InlineData("--token")]
    public void ParseArguments_WithTokenFlag_SetsChangeTokenToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeToken);
    }

    [Theory]
    [InlineData("-un")]
    [InlineData("--username")]
    public void ParseArguments_WithUsernameFlag_SetsChangeUsernameToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeUsername);
    }

    [Theory]
    [InlineData("-pw")]
    [InlineData("--password")]
    public void ParseArguments_WithPasswordFlag_SetsChangePasswordToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangePassword);
    }

    [Theory]
    [InlineData("-f")]
    [InlineData("--follow")]
    public void ParseArguments_WithFollowFlag_SetsChangeFollowRedirectsToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeFollowRedirects);
    }

    [Theory]
    [InlineData("-sh")]
    [InlineData("--show-headers")]
    public void ParseArguments_WithShowHeadersFlag_SetsChangeShowHeadersToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeShowHeaders);
    }

    [Theory]
    [InlineData("-se")]
    [InlineData("--show-errors")]
    public void ParseArguments_WithShowErrorsFlag_SetsChangeShowErrorToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeShowError);
    }

    [Theory]
    [InlineData("-sp")]
    [InlineData("--show-progress")]
    public void ParseArguments_WithShowProgressFlag_SetsChangeShowProgressToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeShowProgress);
    }

    [Theory]
    [InlineData("-mt")]
    [InlineData("--max-timeout")]
    public void ParseArguments_WithMaxTimeoutFlag_SetsChangeMaxTimeoutToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeMaxTimeout);
    }

    [Theory]
    [InlineData("-dh")]
    [InlineData("--default-headers")]
    public void ParseArguments_WithDefaultHeadersFlag_SetsChangeDefaultHeadersToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeDefaultHeaders);
    }

    [Theory]
    [InlineData("-qp")]
    [InlineData("--query-parameters")]
    public void ParseArguments_WithQueryParametersFlag_SetsChangeQueryParametersToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeQueryParameters);
    }

    [Theory]
    [InlineData("-gk")]
    [InlineData("--gemini-key")]
    public void ParseArguments_WithGeminiKeyFlag_SetsChangeGeminiApiKeyToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeGeminiApiKey);
    }

    [Theory]
    [InlineData("-ok")]
    [InlineData("--openai-key")]
    public void ParseArguments_WithOpenAiKeyFlag_SetsChangeOpenAiApiKeyToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeOpenAiApiKey);
    }

    [Theory]
    [InlineData("-ad")]
    [InlineData("--ai-debugging")]
    public void ParseArguments_WithAiDebuggingFlag_SetsChangeAiDebuggingToTrue(string flag)
    {
        var args = new[] { flag };

        var service = new ArgumentService(args);

        Assert.True(service.ChangeAiDebugging);
    }

    [Theory]
    [InlineData("-ai", "generate a POST request")]
    [InlineData("--ai-prompt", "create a GET request to /users")]
    public void ParseArguments_WithAiPromptFlag_SetsUseAiPromptAndAiPrompt(string flag, string prompt)
    {
        var args = new[] { flag, prompt };

        var service = new ArgumentService(args);

        Assert.True(service.UseAiPrompt);
        Assert.Equal(prompt, service.AiPrompt);
    }

    [Theory]
    [InlineData("-p", "/api/users")]
    [InlineData("--path", "/products/123")]
    public void ParseArguments_WithPathFlag_SetsPath(string flag, string path)
    {
        var args = new[] { flag, path };

        var service = new ArgumentService(args);

        Assert.Equal(path, service.Path);
    }

    [Fact]
    public void ParseArguments_WithMultipleFlags_SetsAllCorrectly()
    {
        var args = new[] { "-e", "-s", "-o", "-p", "/api/test" };

        var service = new ArgumentService(args);

        Assert.True(service.ExecuteRequest);
        Assert.True(service.SaveCurl);
        Assert.True(service.SaveOutput);
        Assert.Equal("/api/test", service.Path);
    }

    [Fact]
    public void ParseArguments_WithMixedCaseFlags_ParsesCorrectly()
    {
        var args = new[] { "-H", "-V", "-E" };

        var service = new ArgumentService(args);

        Assert.True(service.ShowHelp);
        Assert.True(service.ShowVersion);
        Assert.True(service.ExecuteRequest);
    }

    [Fact]
    public void ParseArguments_WithAiPromptButNoValue_DoesNotSetAiPrompt()
    {
        var args = new[] { "-ai" };

        var service = new ArgumentService(args);

        Assert.False(service.UseAiPrompt);
        Assert.Null(service.AiPrompt);
    }

    [Fact]
    public void ParseArguments_WithPathButNoValue_DoesNotSetPath()
    {
        var args = new[] { "-p" };

        var service = new ArgumentService(args);

        Assert.Null(service.Path);
    }

    [Fact]
    public void ParseArguments_WithUnknownFlags_IgnoresThem()
    {
        var args = new[] { "-x", "--unknown", "-e" };

        var service = new ArgumentService(args);

        Assert.True(service.ExecuteRequest);
        Assert.False(service.ShowHelp);
    }

    [Fact]
    public void ParseArguments_WithLongAndShortFormsTogether_ParsesAll()
    {
        var args = new[] { "-e", "--save", "-c", "--version" };

        var service = new ArgumentService(args);

        Assert.True(service.ExecuteRequest);
        Assert.True(service.SaveCurl);
        Assert.True(service.ShowSettings);
        Assert.True(service.ShowVersion);
    }
}
