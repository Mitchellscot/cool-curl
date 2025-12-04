using CoolCurl.Models;

namespace CoolCurl.Tests;

public class AuthTypeTests
{
    [Fact]
    public void AuthType_HasNoneValue()
    {
        var authType = AuthType.None;
        Assert.Equal(AuthType.None, authType);
    }

    [Fact]
    public void AuthType_HasBearerTokenValue()
    {
        var authType = AuthType.BearerToken;
        Assert.Equal(AuthType.BearerToken, authType);
    }

    [Fact]
    public void AuthType_HasBasicAuthValue()
    {
        var authType = AuthType.BasicAuth;
        Assert.Equal(AuthType.BasicAuth, authType);
    }

    [Fact]
    public void AuthType_HasJwtBearerValue()
    {
        var authType = AuthType.JwtBearer;
        Assert.Equal(AuthType.JwtBearer, authType);
    }

    [Fact]
    public void AuthType_CanConvertToInt()
    {
        var noneValue = (int)AuthType.None;
        var bearerValue = (int)AuthType.BearerToken;
        var basicValue = (int)AuthType.BasicAuth;
        var jwtValue = (int)AuthType.JwtBearer;

        Assert.Equal(0, noneValue);
        Assert.Equal(1, bearerValue);
        Assert.Equal(2, basicValue);
        Assert.Equal(3, jwtValue);
    }

    [Fact]
    public void AuthType_CanConvertFromInt()
    {
        var none = (AuthType)0;
        var bearer = (AuthType)1;
        var basic = (AuthType)2;
        var jwt = (AuthType)3;

        Assert.Equal(AuthType.None, none);
        Assert.Equal(AuthType.BearerToken, bearer);
        Assert.Equal(AuthType.BasicAuth, basic);
        Assert.Equal(AuthType.JwtBearer, jwt);
    }

    [Theory]
    [InlineData(AuthType.None)]
    [InlineData(AuthType.BearerToken)]
    [InlineData(AuthType.BasicAuth)]
    [InlineData(AuthType.JwtBearer)]
    public void AuthType_AllValuesAreDefined(AuthType authType)
    {
        Assert.True(Enum.IsDefined(typeof(AuthType), authType));
    }

    [Fact]
    public void AuthType_ToString_ReturnsName()
    {
        Assert.Equal("None", AuthType.None.ToString());
        Assert.Equal("BearerToken", AuthType.BearerToken.ToString());
        Assert.Equal("BasicAuth", AuthType.BasicAuth.ToString());
        Assert.Equal("JwtBearer", AuthType.JwtBearer.ToString());
    }
}
