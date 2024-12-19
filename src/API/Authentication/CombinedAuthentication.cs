using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API.Authentication;

class CombinedAuthentication(
  IOptionsMonitor<AuthenticationSchemeOptions> options,
  ILoggerFactory logger,
  UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
  public const string SchemeName = "CombinedAuthentication";

  private readonly string[] _authSchemes = [
    BasicAuthentication.SchemeName,
    ApiKeyAuthentication.SchemeName,
    ClientCredentialsAuthentication.SchemeName,
    JwtBearerDefaults.AuthenticationScheme,
  ];

  protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    foreach (var scheme in _authSchemes)
    {
      var result = await Context.AuthenticateAsync(scheme);
      if (result.Succeeded)
      {
        return result;
      }
    }

    return AuthenticateResult.Fail("Invalid authentication information.");
  }
}