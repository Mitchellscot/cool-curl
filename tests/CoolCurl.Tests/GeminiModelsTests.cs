using CoolCurl.Models;
using System.Text.Json;

namespace CoolCurl.Tests;

public class GeminiModelsTests
{
    [Fact]
    public void GeminiResponse_CanBeInstantiated()
    {
        var response = new GeminiResponse();
        
        Assert.NotNull(response);
        Assert.Null(response.Candidates);
    }

    [Fact]
    public void GeminiResponse_CanSetCandidates()
    {
        var response = new GeminiResponse
        {
            Candidates = new[] { new GeminiCandidate() }
        };

        Assert.NotNull(response.Candidates);
        Assert.Single(response.Candidates);
    }

    [Fact]
    public void GeminiCandidate_CanBeInstantiated()
    {
        var candidate = new GeminiCandidate();
        
        Assert.NotNull(candidate);
        Assert.Null(candidate.Content);
    }

    [Fact]
    public void GeminiCandidate_CanSetContent()
    {
        var candidate = new GeminiCandidate
        {
            Content = new GeminiContent()
        };

        Assert.NotNull(candidate.Content);
    }

    [Fact]
    public void GeminiContent_CanBeInstantiated()
    {
        var content = new GeminiContent();
        
        Assert.NotNull(content);
        Assert.Null(content.Parts);
    }

    [Fact]
    public void GeminiContent_CanSetParts()
    {
        var content = new GeminiContent
        {
            Parts = new[] { new GeminiPart() }
        };

        Assert.NotNull(content.Parts);
        Assert.Single(content.Parts);
    }

    [Fact]
    public void GeminiPart_CanBeInstantiated()
    {
        var part = new GeminiPart();
        
        Assert.NotNull(part);
        Assert.Null(part.Text);
    }

    [Fact]
    public void GeminiPart_CanSetText()
    {
        var part = new GeminiPart
        {
            Text = "test text"
        };

        Assert.Equal("test text", part.Text);
    }

    [Fact]
    public void GeminiResponse_CanDeserializeFromJson()
    {
        var json = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": ""curl command here""
                            }
                        ]
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<GeminiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Candidates);
        Assert.Single(response.Candidates);
        Assert.NotNull(response.Candidates[0].Content);
        Assert.NotNull(response.Candidates[0].Content.Parts);
        Assert.Single(response.Candidates[0].Content.Parts);
        Assert.Equal("curl command here", response.Candidates[0].Content.Parts[0].Text);
    }

    [Fact]
    public void GeminiResponse_CanDeserializeEmptyCandidates()
    {
        var json = @"{""candidates"": []}";

        var response = JsonSerializer.Deserialize<GeminiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Candidates);
        Assert.Empty(response.Candidates);
    }

    [Fact]
    public void GeminiResponse_CanSerializeToJson()
    {
        var response = new GeminiResponse
        {
            Candidates = new[]
            {
                new GeminiCandidate
                {
                    Content = new GeminiContent
                    {
                        Parts = new[]
                        {
                            new GeminiPart { Text = "test response" }
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(response);

        Assert.Contains("\"candidates\"", json);
        Assert.Contains("\"content\"", json);
        Assert.Contains("\"parts\"", json);
        Assert.Contains("\"text\"", json);
        Assert.Contains("test response", json);
    }

    [Fact]
    public void GeminiResponse_WithMultipleCandidates_Deserializes()
    {
        var json = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            { ""text"": ""first"" }
                        ]
                    }
                },
                {
                    ""content"": {
                        ""parts"": [
                            { ""text"": ""second"" }
                        ]
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<GeminiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Candidates);
        Assert.Equal(2, response.Candidates.Length);
        Assert.Equal("first", response.Candidates[0].Content?.Parts?[0].Text);
        Assert.Equal("second", response.Candidates[1].Content?.Parts?[0].Text);
    }

    [Fact]
    public void GeminiContent_WithMultipleParts_Deserializes()
    {
        var json = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            { ""text"": ""part1"" },
                            { ""text"": ""part2"" },
                            { ""text"": ""part3"" }
                        ]
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<GeminiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Candidates);
        var parts = response.Candidates[0].Content?.Parts;
        Assert.NotNull(parts);
        Assert.Equal(3, parts.Length);
        Assert.Equal("part1", parts[0].Text);
        Assert.Equal("part2", parts[1].Text);
        Assert.Equal("part3", parts[2].Text);
    }

    [Fact]
    public void GeminiResponse_WithNullValues_Deserializes()
    {
        var json = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            { ""text"": null }
                        ]
                    }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<GeminiResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Candidates);
        Assert.Null(response.Candidates[0].Content?.Parts?[0].Text);
    }

    [Fact]
    public void GeminiPart_WithEmptyText_Works()
    {
        var part = new GeminiPart { Text = "" };
        
        Assert.Equal("", part.Text);
    }
}
