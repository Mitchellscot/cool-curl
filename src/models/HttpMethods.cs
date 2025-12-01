namespace CoolCurl.Models;

public static class HttpMethods
{
    public const string GET = "GET";
    public const string POST = "POST";
    public const string PUT = "PUT";
    public const string DELETE = "DELETE";
    public const string PATCH = "PATCH";
    public const string HEAD = "HEAD";
    public const string OPTIONS = "OPTIONS";

    public static readonly List<string> ValidMethods = new()
    {
        GET,
        POST,
        PUT,
        DELETE,
        PATCH,
        HEAD,
        OPTIONS
    };

    public static bool IsValid(string method)
    {
        return ValidMethods.Contains(method.ToUpper());
    }

    public static string Normalize(string method)
    {
        return method.ToUpper();
    }
}
