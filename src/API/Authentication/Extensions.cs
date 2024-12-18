using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Authentication;

public static class Extensions
{
  public static IServiceCollection AddAuthN(this IServiceCollection services)
  {
    services.AddAuthentication()
      .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(BasicAuthentication.SchemeName, null)
      .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthentication>(ApiKeyAuthentication.SchemeName, null)
      .AddScheme<AuthenticationSchemeOptions, ClientCredentialsAuthentication>(ClientCredentialsAuthentication.SchemeName, null)
      .AddJwtBearer(options => 
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAuthenticator.Secret)),
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidIssuer = JwtAuthenticator.Issuer,
          ValidAudience = JwtAuthenticator.Audience,
          ClockSkew = TimeSpan.Zero,
        })
      .AddScheme<AuthenticationSchemeOptions, CombinedAuthentication>(CombinedAuthentication.SchemeName, null);

    services.AddSingleton<JwtAuthenticator>();

    return services;
  }

  public static IServiceCollection AddAuthZ(this IServiceCollection services)
  {
    services
      .AddAuthorizationBuilder()
      .AddPolicy(BasicAuthentication.SchemeName, policy =>
      {
        policy.AuthenticationSchemes.Add(BasicAuthentication.SchemeName);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier);
      })
      .AddPolicy(ApiKeyAuthentication.SchemeName, policy =>
      {
        policy.AuthenticationSchemes.Add(ApiKeyAuthentication.SchemeName);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier);
      })
      .AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
      {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier);
      })
      .AddPolicy(ClientCredentialsAuthentication.SchemeName, policy =>
      {
        policy.AuthenticationSchemes.Add(ClientCredentialsAuthentication.SchemeName);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier);
      })
      .AddPolicy(CombinedAuthentication.SchemeName, policy =>
      {
        policy.AuthenticationSchemes.Add(CombinedAuthentication.SchemeName);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier);
      });

    return services;
  }
}