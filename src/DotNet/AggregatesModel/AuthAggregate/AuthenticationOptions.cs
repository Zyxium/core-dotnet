using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Core.DotNet.AggregatesModel.AuthAggregate;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class BasicAuthenticationPostConfigureOptions : IPostConfigureOptions<BasicAuthenticationOptions>
{
    public void PostConfigure(string name, BasicAuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Username) ||
            string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException("Username and Password must be provided in options.");
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ApiKeys { get; set; }
}

public class ApiKeyAuthenticationPostConfigureOptions : IPostConfigureOptions<ApiKeyAuthenticationOptions>
{
    public void PostConfigure(string name, ApiKeyAuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKeys))
            throw new InvalidOperationException("ApiKeys must be provided in options.");
    }
}

public class JwtBearerAuthenticationOptions : AuthenticationSchemeOptions
{
    public string JWKsEndpoint { get; set; }
}

public class JwtBearerAuthenticationPostConfigureOptions : IPostConfigureOptions<JwtBearerAuthenticationOptions>
{
    public void PostConfigure(string name, JwtBearerAuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.JWKsEndpoint))
            throw new InvalidOperationException("JWKsEndpoint must be provided in options.");
    }
}