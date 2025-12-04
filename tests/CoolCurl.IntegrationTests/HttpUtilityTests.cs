using CoolCurl.Infrastructure;
using CoolCurl.Models;

namespace CoolCurl.IntegrationTests;

public class HttpUtilityTests
{
    [Fact]
    public void BuildUrl_WithBaseUrlOnly_ReturnsBaseUrl()
    {
        // Arrange
        var settings = new Settings { BaseUrl = "https://api.example.com" };

        // Act
        var result = HttpUtility.BuildUrl(settings);

        // Assert
        Assert.Equal("https://api.example.com", result);
    }

    [Fact]
    public void BuildUrl_WithBaseUrlAndPath_ReturnsCombinedUrl()
    {
        // Arrange
        var settings = new Settings { BaseUrl = "https://api.example.com" };

        // Act
        var result = HttpUtility.BuildUrl(settings, "/users");

        // Assert
        Assert.Equal("https://api.example.com/users", result);
    }

    [Fact]
    public void BuildUrl_WithTrailingSlashInBaseUrl_RemovesTrailingSlash()
    {
        // Arrange
        var settings = new Settings { BaseUrl = "https://api.example.com/" };

        // Act
        var result = HttpUtility.BuildUrl(settings, "/users");

        // Assert
        Assert.Equal("https://api.example.com/users", result);
    }

    [Fact]
    public void BuildUrl_WithPathWithoutLeadingSlash_AddsLeadingSlash()
    {
        // Arrange
        var settings = new Settings { BaseUrl = "https://api.example.com" };

        // Act
        var result = HttpUtility.BuildUrl(settings, "users");

        // Assert
        Assert.Equal("https://api.example.com/users", result);
    }

    [Fact]
    public void BuildUrl_WithQueryParameters_AppendsQueryString()
    {
        // Arrange
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com",
            QueryParameters = new Dictionary<string, string>
            {
                { "page", "1" },
                { "limit", "10" }
            }
        };

        // Act
        var result = HttpUtility.BuildUrl(settings, "/users");

        // Assert
        Assert.Equal("https://api.example.com/users?page=1&limit=10", result);
    }

    [Fact]
    public void BuildUrl_WithExistingQueryStringAndParameters_AppendsWithAmpersand()
    {
        // Arrange
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com/users?sort=name",
            QueryParameters = new Dictionary<string, string>
            {
                { "page", "1" }
            }
        };

        // Act
        var result = HttpUtility.BuildUrl(settings);

        // Assert
        Assert.Equal("https://api.example.com/users?sort=name&page=1", result);
    }

    [Fact]
    public void BuildUrl_WithSpecialCharactersInParameters_EscapesCharacters()
    {
        // Arrange
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com",
            QueryParameters = new Dictionary<string, string>
            {
                { "name", "John Doe" },
                { "email", "john@example.com" }
            }
        };

        // Act
        var result = HttpUtility.BuildUrl(settings, "/users");

        // Assert
        Assert.Contains("John%20Doe", result);
        Assert.Contains("john%40example.com", result);
    }

    [Fact]
    public void BuildUrl_WithRecentPathsAndNoOverride_UsesFirstRecentPath()
    {
        // Arrange
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com",
            RecentPaths = new List<string> { "/users", "/posts" }
        };

        // Act
        var result = HttpUtility.BuildUrl(settings);

        // Assert
        Assert.Equal("https://api.example.com/users", result);
    }

    [Fact]
    public void BuildUrl_WithPathOverride_IgnoresRecentPaths()
    {
        // Arrange
        var settings = new Settings
        {
            BaseUrl = "https://api.example.com",
            RecentPaths = new List<string> { "/users", "/posts" }
        };

        // Act
        var result = HttpUtility.BuildUrl(settings, "/comments");

        // Assert
        Assert.Equal("https://api.example.com/comments", result);
    }

    [Theory]
    [InlineData("GET", "GET")]
    [InlineData("POST", "POST")]
    [InlineData("PUT", "PUT")]
    [InlineData("DELETE", "DELETE")]
    [InlineData("PATCH", "PATCH")]
    [InlineData("HEAD", "HEAD")]
    [InlineData("OPTIONS", "OPTIONS")]
    public void GetHttpMethod_WithValidMethod_ReturnsCorrectHttpMethod(string input, string expected)
    {
        // Act
        var result = HttpUtility.GetHttpMethod(input);

        // Assert
        Assert.Equal(expected, result.Method);
    }

    [Theory]
    [InlineData("get")]
    [InlineData("post")]
    [InlineData("put")]
    [InlineData("delete")]
    public void GetHttpMethod_WithLowercaseMethod_ReturnsCorrectHttpMethod(string input)
    {
        // Act
        var result = HttpUtility.GetHttpMethod(input);

        // Assert
        Assert.Equal(input.ToUpper(), result.Method);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("INVALID")]
    public void GetHttpMethod_WithInvalidMethod_ReturnsGetMethod(string? input)
    {
        // Act
        var result = HttpUtility.GetHttpMethod(input);

        // Assert
        Assert.Equal("GET", result.Method);
    }

    [Fact]
    public void SaveToFile_CreatesFileInCorrectDirectory()
    {
        // Arrange
        var content = "Test content";
        var subdirectory = "test-output";
        var filePrefix = "test";
        var successMessage = "File saved";

        // Capture console output
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            HttpUtility.SaveToFile(content, subdirectory, filePrefix, successMessage);

            var output = stringWriter.ToString();

            // Assert
            Assert.Contains(successMessage, output);
            Assert.Contains(".cool-curl", output);
            Assert.Contains(subdirectory, output);
            Assert.Contains(filePrefix, output);

            // Cleanup - find and delete the created file
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outputDirectory = Path.Combine(homeDirectory, ".cool-curl", subdirectory);

            if (Directory.Exists(outputDirectory))
            {
                var files = Directory.GetFiles(outputDirectory, $"{filePrefix}_*.txt");
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
    public void SaveToFile_WithInvalidPath_HandlesErrorGracefully()
    {
        // Arrange
        var content = "Test content";
        var subdirectory = new string('x', 300); // Extremely long path
        var filePrefix = "test";
        var successMessage = "File saved";

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            HttpUtility.SaveToFile(content, subdirectory, filePrefix, successMessage);

            var output = stringWriter.ToString();

            // Assert - should contain error message
            Assert.Contains("Error", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void SaveToFile_CreatesTimestampedFilename()
    {
        // Arrange
        var content = "Test content";
        var subdirectory = "test-timestamp";
        var filePrefix = "test";
        var successMessage = "File saved";

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            HttpUtility.SaveToFile(content, subdirectory, filePrefix, successMessage);

            var output = stringWriter.ToString();

            // Assert - filename should contain timestamp pattern
            Assert.Matches(@"test_\d{8}_\d{6}\.txt", output);

            // Cleanup
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outputDirectory = Path.Combine(homeDirectory, ".cool-curl", subdirectory);

            if (Directory.Exists(outputDirectory))
            {
                var files = Directory.GetFiles(outputDirectory, $"{filePrefix}_*.txt");
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
    public void SaveToFile_WritesCorrectContent()
    {
        // Arrange
        var content = "Test content with special characters: !@#$%^&*()";
        var subdirectory = "test-content";
        var filePrefix = "test";
        var successMessage = "File saved";

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            HttpUtility.SaveToFile(content, subdirectory, filePrefix, successMessage);

            var output = stringWriter.ToString();

            // Read the created file
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outputDirectory = Path.Combine(homeDirectory, ".cool-curl", subdirectory);
            var files = Directory.GetFiles(outputDirectory, $"{filePrefix}_*.txt");

            Assert.Single(files);
            var writtenContent = File.ReadAllText(files[0]);

            // Assert
            Assert.Equal(content, writtenContent);

            // Cleanup
            File.Delete(files[0]);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
