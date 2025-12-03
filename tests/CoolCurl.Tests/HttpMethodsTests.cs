using CoolCurl.Models;

namespace CoolCurl.Tests;

public class HttpMethodsTests
{
    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    [InlineData("HEAD")]
    [InlineData("OPTIONS")]
    public void IsValid_WithValidMethod_ReturnsTrue(string method)
    {
        var result = HttpMethods.IsValid(method);

        Assert.True(result);
    }

    [Theory]
    [InlineData("get")]
    [InlineData("post")]
    [InlineData("Put")]
    [InlineData("DeLeTe")]
    public void IsValid_WithValidMethodMixedCase_ReturnsTrue(string method)
    {
        var result = HttpMethods.IsValid(method);

        Assert.True(result);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("TRACE")]
    [InlineData("CONNECT")]
    [InlineData("")]
    [InlineData("get post")]
    public void IsValid_WithInvalidMethod_ReturnsFalse(string method)
    {
        var result = HttpMethods.IsValid(method);

        Assert.False(result);
    }

    [Theory]
    [InlineData("get", "GET")]
    [InlineData("POST", "POST")]
    [InlineData("Put", "PUT")]
    [InlineData("DeLeTe", "DELETE")]
    [InlineData("patch", "PATCH")]
    public void Normalize_ReturnsUppercaseMethod(string input, string expected)
    {
        var result = HttpMethods.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValidMethods_ContainsSevenMethods()
    {
        Assert.Equal(7, HttpMethods.ValidMethods.Count);
    }

    [Fact]
    public void ValidMethods_ContainsExpectedMethods()
    {
        var expectedMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };

        foreach (var method in expectedMethods)
        {
            Assert.Contains(method, HttpMethods.ValidMethods);
        }
    }
}
