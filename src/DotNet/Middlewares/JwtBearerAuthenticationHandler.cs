using Core.DotNet.AggregatesModel.AuthAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Core.DotNet.Middlewares;

public class JwtBearerAuthenticationHandler : AuthenticationHandler<JwtBearerAuthenticationOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "Bearer ";
    private const string JWKsMemoryCacheKey = "auth_jwks";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;

    public JwtBearerAuthenticationHandler(
        IOptionsMonitor<JwtBearerAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache)
        : base(options, logger, encoder, clock)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            return AuthenticateResult.Fail("Missing Authorization Header");

        try
        {
            var bearer = Request.Headers[AuthorizationHeaderName];

            if (string.IsNullOrWhiteSpace(bearer) || !bearer.ToString().StartsWith(BasicSchemeName))
                return AuthenticateResult.Fail("Invalid Authorization header");

            var token = bearer.ToString().Remove(0, 7);
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = "https://account.hivecorps.com",
                ClockSkew = TimeSpan.FromMinutes(2),
                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                {
                    if (_memoryCache.TryGetValue(JWKsMemoryCacheKey, out List<SecurityKey> securityKeys))
                        return securityKeys;

                    var request = new HttpRequestMessage(HttpMethod.Get, Options.JWKsEndpoint);

                    var client = _httpClientFactory.CreateClient();

                    var response = client.Send(request);

                    if (!response.IsSuccessStatusCode) return new List<JsonWebKey>();

                    var jwks = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    var signingKeys = new JsonWebKeySet(jwks).GetSigningKeys();

                    _memoryCache.Set(JWKsMemoryCacheKey, signingKeys, TimeSpan.FromDays(1));

                    return signingKeys;
                }
            }, out var validatedToken);

            var jwtSecurityToken = (JwtSecurityToken)validatedToken;

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, ((JwtSecurityToken)validatedToken).Payload[JwtRegisteredClaimNames.Sub]?.ToString() ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}