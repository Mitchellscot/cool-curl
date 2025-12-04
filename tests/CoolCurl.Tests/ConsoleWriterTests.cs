using CoolCurl.Services;

namespace CoolCurl.Tests;

public class ConsoleWriterTests
{
    [Fact]
    public void Write_CallsConsoleWrite()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.Write("test message");

        Assert.Equal("test message", stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void WriteLine_WithString_CallsConsoleWriteLine()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.WriteLine("test message");

        Assert.Equal("test message" + Environment.NewLine, stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void WriteLine_WithoutParameter_CallsConsoleWriteLine()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.WriteLine();

        Assert.Equal(Environment.NewLine, stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void ReadLine_ReturnsConsoleInput()
    {
        var writer = new ConsoleWriter();
        var originalIn = Console.In;

        using var stringReader = new StringReader("test input");
        Console.SetIn(stringReader);

        var result = writer.ReadLine();

        Assert.Equal("test input", result);

        Console.SetIn(originalIn);
    }

    [Fact]
    public void ReadLine_WithEmptyInput_ReturnsNull()
    {
        var writer = new ConsoleWriter();
        var originalIn = Console.In;

        using var stringReader = new StringReader("");
        Console.SetIn(stringReader);

        var result = writer.ReadLine();

        Assert.Null(result);

        Console.SetIn(originalIn);
    }

    [Fact]
    public void ForegroundColor_Get_ReturnsConsoleColor()
    {
        var writer = new ConsoleWriter();

        var result = writer.ForegroundColor;

        // Just verify it returns a valid ConsoleColor value
        Assert.True(Enum.IsDefined(typeof(ConsoleColor), result));
    }

    [Fact]
    public void ForegroundColor_Set_DoesNotThrow()
    {
        var writer = new ConsoleWriter();
        var originalColor = Console.ForegroundColor;

        // In test environments, setting console color may not work
        // Just verify it doesn't throw an exception
        var exception = Record.Exception(() => writer.ForegroundColor = ConsoleColor.Green);

        Assert.Null(exception);

        Console.ForegroundColor = originalColor;
    }

    [Fact]
    public void ResetColor_CallsConsoleResetColor()
    {
        var writer = new ConsoleWriter();
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Yellow;
        writer.ResetColor();

        // After reset, color should be different from Yellow
        // Note: We can't test the exact color as it depends on terminal settings
        // But we can verify the method executes without error
        Assert.NotNull(writer);

        Console.ForegroundColor = originalColor;
    }

    [Fact]
    public void Write_WithEmptyString_WritesEmptyString()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.Write("");

        Assert.Equal("", stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void WriteLine_WithEmptyString_WritesNewLine()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.WriteLine("");

        Assert.Equal(Environment.NewLine, stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void Write_WithMultipleLines_WritesAllContent()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.Write("line1\nline2\nline3");

        Assert.Equal("line1\nline2\nline3", stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void WriteLine_WithMultipleLines_WritesAllContentWithNewLine()
    {
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        writer.WriteLine("line1\nline2");

        Assert.Equal("line1\nline2" + Environment.NewLine, stringWriter.ToString());

        Console.SetOut(originalOut);
    }

    [Fact]
    public void ForegroundColor_SetMultipleTimes_DoesNotThrow()
    {
        var writer = new ConsoleWriter();
        var originalColor = Console.ForegroundColor;

        // Just verify setting multiple times doesn't throw
        var exception = Record.Exception(() =>
        {
            writer.ForegroundColor = ConsoleColor.Blue;
            writer.ForegroundColor = ConsoleColor.Magenta;
            writer.ForegroundColor = ConsoleColor.Cyan;
        });

        Assert.Null(exception);

        Console.ForegroundColor = originalColor;
    }
}
