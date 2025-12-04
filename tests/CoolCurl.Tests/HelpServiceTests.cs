using CoolCurl.Services;

namespace CoolCurl.Tests;

public class HelpServiceTests
{
    [Fact]
    public void DisplayHelp_ExecutesWithoutException()
    {
        // Redirect console output to capture it
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            // Should not throw any exceptions
            HelpService.DisplayHelp();

            var output = writer.ToString();

            // Verify the output is not empty
            Assert.False(string.IsNullOrWhiteSpace(output));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsHelpHeader()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("CoolCurl Help", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsBasicOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--help", output);
            Assert.Contains("--version", output);
            Assert.Contains("--config", output);
            Assert.Contains("--reset", output);
            Assert.Contains("--execute", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsConfigurationOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--method", output);
            Assert.Contains("--url", output);
            Assert.Contains("--auth", output);
            Assert.Contains("--token", output);
            Assert.Contains("--username", output);
            Assert.Contains("--password", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsAiFeatures()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("AI Features", output);
            Assert.Contains("--ai-prompt", output);
            Assert.Contains("--gemini-key", output);
            Assert.Contains("--openai-key", output);
            Assert.Contains("--ai-debugging", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsShortFormOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            // Check for short form flags
            Assert.Contains("-h,", output);
            Assert.Contains("-v,", output);
            Assert.Contains("-c,", output);
            Assert.Contains("-e,", output);
            Assert.Contains("-m,", output);
            Assert.Contains("-u,", output);
            Assert.Contains("-a,", output);
            Assert.Contains("-t,", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsHeaderManagementOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--default-headers", output);
            Assert.Contains("--query-parameters", output);
            Assert.Contains("--show-headers", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsFollowRedirectsOption()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--follow", output);
            Assert.Contains("redirects", output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsTimeoutOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--max-timeout", output);
            Assert.Contains("timeout", output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsSaveOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--save", output);
            Assert.Contains("--output", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsGettingStartedSection()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("Getting Started", output);
            Assert.Contains("first time", output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsBasicAuthOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("-un,", output);
            Assert.Contains("-pw,", output);
            Assert.Contains("username", output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("password", output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsProgressAndErrorOptions()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--show-progress", output);
            Assert.Contains("--show-errors", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsPathOption()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("--path", output);
            Assert.Contains("<path>", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_ContainsAiPromptExample()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("Example:", output);
            Assert.Contains("GET request", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void DisplayHelp_MentionsGoogleGeminiAndOpenAI()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            HelpService.DisplayHelp();
            var output = writer.ToString();

            Assert.Contains("Google Gemini", output);
            Assert.Contains("OpenAI", output);
            Assert.Contains("gpt-4o-mini", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
