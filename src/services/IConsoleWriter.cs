namespace CoolCurl.Services;

public interface IConsoleWriter
{
    void Write(string value);
    void WriteLine(string value);
    void WriteLine();
    string? ReadLine();
    ConsoleColor ForegroundColor { get; set; }
    void ResetColor();
}
