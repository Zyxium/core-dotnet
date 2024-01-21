using Core.DotNet.AggregatesModel.AuthAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Core.DotNet.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "Basic";

    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
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
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue authHeader))
                return AuthenticateResult.NoResult();

            if (!BasicSchemeName.Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.NoResult();

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);

            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Invalid Authorization header");

            var username = credentials[0];
            var password = credentials[1];

            if (!(Options.Username == username &&
                  Options.Password == password))
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