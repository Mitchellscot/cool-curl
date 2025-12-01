namespace CoolCurl.Models;

public class Settings
{
    public string? BaseUrl { get; set; }
    public string? DefaultMethod { get; set; }
    public List<string> RecentPaths { get; set; } = new();
    public AuthType AuthType { get; set; } = AuthType.None;
    public string? AuthToken { get; set; }
    public string? BasicAuthUsername { get; set; }
    public string? BasicAuthPassword { get; set; }
    public bool FollowRedirects { get; set; } = true;
    public bool ShowProgress { get; set; } = false;
    public bool ShowError { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;
    public int? MaxTimeSeconds { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; set; } = new()
    {
        { "Accept", "application/json" },
        { "Content-Type", "application/json" },
        { "User-Agent", "CoolCurl/1.0" }
    };
}
