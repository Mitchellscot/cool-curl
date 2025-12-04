namespace CoolCurl.Services;

public class ConsoleWriter : IConsoleWriter
{
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public void Write(string value)
    {
        Console.Write(value);
    }

    public void WriteLine(string value)
    {
        Console.WriteLine(value);
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    public string? ReadLine()
    {
        return Console.ReadLine();
    }

    public void ResetColor()
    {
        Console.ResetColor();
    }
}
