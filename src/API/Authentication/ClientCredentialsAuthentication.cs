

namespace API.Authentication;

public class ClientCredentialsAuthentication(
  IOptionsMonitor<AuthenticationSchemeOptions> options, 
  ILoggerFactory logger, 
  UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
  public const string SchemeName = "Client Credentials";

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var clientId = string.Empty;
    var clientSecret = string.Empty;

    var contentType = Request.ContentType;

    if (contentType is null)
    {
      return AuthenticateResult.Fail("Invalid client credentials");
    }

    if (contentType.Contains("application/x-www-form-urlencoded", StringComparison.InvariantCultureIgnoreCase))
    {
      var form = await Request.ReadFormAsync();
      clientId = form.TryGetValue("client_id", out var id) ? id : string.Empty;
      clientSecret = form.TryGetValue("client_secret", out var secret) ? secret : string.Empty;
    }
    else if (contentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase))
    {
      var body = await Request.ReadFromJsonAsync<Dictionary<string, string>>();

      if (body is not null)
      {
        clientId = body.TryGetValue("client_id", out var id) ? id : string.Empty;
        clientSecret = body.TryGetValue("client_secret", out var secret) ? secret : string.Empty;
      }
    }

    if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
    {
      return AuthenticateResult.Fail("Invalid client credentials");
    }

    if (clientId is not "client" || clientSecret is not "secret")
    {
      return AuthenticateResult.Fail("Invalid client credentials");
    }

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, clientId),
      new Claim(ClaimTypes.Name, clientId),
    };

    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return AuthenticateResult.Success(ticket);
  }
}