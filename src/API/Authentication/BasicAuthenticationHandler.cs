namespace API.Authentication;

class BasicAuthenticationHandler(
  IOptionsMonitor<AuthenticationSchemeOptions> options,
  ILoggerFactory logger,
  UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var authHeader = Request.Headers.Authorization.FirstOrDefault();

    if (authHeader is null)
    {
      return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing"));
    }

    if (authHeader.StartsWith("Basic ") is false)
    {
      return Task.FromResult(AuthenticateResult.Fail("Authorization header is not Basic"));
    }

    var headerParts = authHeader.Split(' ', 2);

    if (headerParts.Length < 2)
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header"));
    }

    var encodedCredentials = headerParts[1];

    var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
    var credentials = decodedCredentials.Split(':', 2);

    if (credentials.Length < 2)
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
    }

    var username = credentials[0];
    var password = credentials[1];

    if (username is not "admin" || password is not "password")
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
    }

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, username),
      new Claim(ClaimTypes.Name, username),
    };

    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}