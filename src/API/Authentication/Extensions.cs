using API.Common;

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

  public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
  {
    var group = app.MapGroup(string.Empty).WithTags("Authentication");

    group
      .MapGet("/no-auth", () => Results.Ok(new SuccessResponse("You are in!")))
      .WithName("NoAuth")
      .WithSummary("No Authentication")
      .WithDescription("Allows sending a request without authentication and get a response back.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK);

    group
      .MapGet("/basic", (HttpContext context) =>
      {
        var user = context.User.Identity?.Name;
        return Results.Ok(new SuccessResponse($"Hello, {user}!"));
      })
      .RequireAuthorization(BasicAuthentication.SchemeName)
      .WithName("Basic")
      .WithSummary("Basic Authentication")
      .WithDescription("Simulates an endpoint that requires basic authentication. Use 'admin' as username and 'password' as password.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("/api-key", (HttpContext context) =>
      {
        var user = context.User.Identity?.Name;
        return Results.Ok(new SuccessResponse($"Hello, {user}!"));
      })
      .RequireAuthorization(ApiKeyAuthentication.SchemeName)
      .WithName("ApiKey")
      .WithSummary("API Key Authentication")
      .WithDescription("Simulates an endpoint that requires API key authentication. Use 'api-key' as the API key.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("/bearer", (HttpContext context) =>
      {
        var user = context.User.Identity?.Name;
        return Results.Ok(new SuccessResponse($"Hello, {user}!"));
      })
      .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme)
      .WithName("Bearer")
      .WithSummary("Bearer Authentication")
      .WithDescription("Simulates an endpoint that requires bearer token authentication. Use the token generated from the 'Generate Token' endpoint.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapPost("/generate-token", ([FromBody] LoginRequest loginRequest, HttpContext context, [FromServices] JwtAuthenticator jwtAuth) =>
      {
        if (loginRequest.IsValid() is false)
        {
          return Results.ValidationProblem(new Dictionary<string, string[]>()
          {
            { "login", ["Invalid login request. Username and password are required."] }
          });
        }

        if (loginRequest.Username != "admin" || loginRequest.Password != "password")
        {
          return Results.Problem(
            detail: "Invalid username or password.",
            statusCode: StatusCodes.Status401Unauthorized
          );
        }

        var (expiresInSecs, value) = jwtAuth.GenerateJwtToken(loginRequest.Username);
        return Results.Ok(new LoginResponse("Bearer", value, expiresInSecs));
      })
      .WithName("GenerateToken")
      .WithSummary("Generate Token")
      .WithDescription("Generates a JWT token using the provided username and password. Use 'admin' as username and 'password' as password.")
      .Produces<LoginResponse>((int)HttpStatusCode.OK)
      .Produces<ValidationProblemDetails>((int)HttpStatusCode.BadRequest)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapPost("/access-token", (HttpContext context, [FromServices] JwtAuthenticator jwtAuth) =>
      {
        var (expiresInSecs, value) = jwtAuth.GenerateJwtToken(context.User.Identity!.Name!);

        return Results.Ok(new LoginResponse("Bearer", value, expiresInSecs));
      })
      .RequireAuthorization(ClientCredentialsAuthentication.SchemeName)
      .WithName("AccessToken")
      .WithSummary("Access Token")
      .WithDescription("Generates an access token using the client credentials flow. Use 'client' as the client ID and 'secret' as the client secret.")
      .Produces<LoginResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("/oauth2", (HttpContext context) =>
      {
        var user = context.User.Identity?.Name;
        return Results.Ok(new SuccessResponse($"Hello, {user}!"));
      })
      .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme)
      .WithName("OAuth2")
      .WithSummary("OAuth2 Protected")
      .WithDescription("Simulates an OAuth2 protected endpoint. Use the token generated from the 'Access Token' endpoint.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }
}