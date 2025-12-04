using CoolCurl.Models;
using System.Text.Json;

namespace CoolCurl.Tests;

public class OpenAiModelsTests
{
    [Fact]
    public void OpenAiResponse_CanBeInstantiated()
    {
        var response = new OpenAiResponse();

        Assert.NotNull(response);
        Assert.Null(response.Choices);
    }

    [Fact]
    public void OpenAiResponse_CanSetChoices()
    {
        var response = new OpenAiResponse
        {
            Choices = new[] { new OpenAiChoice() }
        };

        Assert.NotNull(response.Choices);
        Assert.Single(response.Choices);
    }

    [Fact]
    public void OpenAiChoice_CanBeInstantiated()
    {
        var choice = new OpenAiChoice();

        Assert.NotNull(choice);
        Assert.Null(choice.Message);
    }

    [Fact]
    public void OpenAiChoice_CanSetMessage()
    {
        var choice = new OpenAiChoice
        {
            Message = new OpenAiMessage()
        };

        Assert.NotNull(choice.Message);
    }

    [Fact]
    public void OpenAiMessage_CanBeInstantiated()
    {
        var message = new OpenAiMessage();

        Assert.NotNull(message);
        Assert.Null(message.Content);
    }

    [Fact]
    public void OpenAiMessage_CanSetContent()
    {
        var message = new OpenAiMessage
        {
            Content = "test content"
        };

        Assert.Equal("test content", message.Content);
    }

    [Fact]
    public void OpenAiResponse_CanDeserializeFromJson()
    {
        var json = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": ""curl command here""
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<OpenAiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.Single(response.Choices);
        Assert.NotNull(response.Choices[0].Message);
        Assert.Equal("curl command here", response.Choices[0].Message!.Content);
    }

    [Fact]
    public void OpenAiResponse_CanDeserializeEmptyChoices()
    {
        var json = @"{""choices"": []}";

        var response = JsonSerializer.Deserialize<OpenAiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.Empty(response.Choices);
    }

    [Fact]
    public void OpenAiResponse_CanSerializeToJson()
    {
        var response = new OpenAiResponse
        {
            Choices = new[]
            {
                new OpenAiChoice
                {
                    Message = new OpenAiMessage
                    {
                        Content = "test response"
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(response);

        Assert.Contains("\"choices\"", json);
        Assert.Contains("\"message\"", json);
        Assert.Contains("\"content\"", json);
        Assert.Contains("test response", json);
    }

    [Fact]
    public void OpenAiResponse_WithMultipleChoices_Deserializes()
    {
        var json = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": ""first choice""
                    }
                },
                {
                    ""message"": {
                        ""content"": ""second choice""
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<OpenAiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.Equal(2, response.Choices.Length);
        Assert.Equal("first choice", response.Choices[0].Message?.Content);
        Assert.Equal("second choice", response.Choices[1].Message?.Content);
    }

    [Fact]
    public void OpenAiResponse_WithNullContent_Deserializes()
    {
        var json = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": null
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<OpenAiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Choices);
        Assert.Null(response.Choices[0].Message?.Content);
    }

    [Fact]
    public void OpenAiMessage_WithEmptyContent_Works()
    {
        var message = new OpenAiMessage { Content = "" };

        Assert.Equal("", message.Content);
    }

    [Fact]
    public void OpenAiResponse_WithNullMessage_Works()
    {
        var choice = new OpenAiChoice { Message = null };

        Assert.Null(choice.Message);
    }
}
