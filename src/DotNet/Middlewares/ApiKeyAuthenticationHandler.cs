using Core.DotNet.AggregatesModel.AuthAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Core.DotNet.Middlewares;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string AuthorizationHeaderName = "api_key";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            return AuthenticateResult.Fail("Missing Authorization Header");

        try
        {
            var apiKey = Request.Headers[AuthorizationHeaderName];

            if (string.IsNullOrWhiteSpace(apiKey))
                return AuthenticateResult.Fail("Invalid Authorization header");

            var apiKeys = Options.ApiKeys.Split(',')
                .Select(m => m.Trim())
                .ToList();

            if (!apiKeys.Contains(apiKey))
                return AuthenticateResult.Fail("Invalid Authorization");
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var claims = new[] {
            new Claim(ClaimTypes.Name, "System")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}