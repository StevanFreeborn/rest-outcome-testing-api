namespace API.Authentication;

class ApiKeyAuthentication(
  IOptionsMonitor<AuthenticationSchemeOptions> options,
  ILoggerFactory logger,
  UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
  public const string SchemeName = "API Key";
  public const string ApiKeyHeaderName = "x-api-key";

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader) is false)
    {
      return Task.FromResult(AuthenticateResult.Fail("API key not found."));
    }

    var apiKey = apiKeyHeader.FirstOrDefault();

    if (apiKey is not "api-key")
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
    }

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, "API User"),
      new Claim(ClaimTypes.Name, "API User"),
    };

    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}