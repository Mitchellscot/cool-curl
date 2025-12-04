using System.Text.Json.Serialization;

namespace CoolCurl.Models;

public class OpenAiResponse
{
    [JsonPropertyName("choices")]
    public OpenAiChoice[]? Choices { get; set; }
}

public class OpenAiChoice
{
    [JsonPropertyName("message")]
    public OpenAiMessage? Message { get; set; }
}

public class OpenAiMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
