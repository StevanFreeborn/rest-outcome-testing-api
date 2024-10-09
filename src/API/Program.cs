
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

const string jwtSecret = "This is a secret key for JWT token generation.";
const string jwtIssuer = "TestingAPI";
const string jwtAudience = "Onspring";

builder.Services.AddAuthentication()
  .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(BasicAuthentication.SchemeName, null)
  .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthentication>(ApiKeyAuthentication.SchemeName, null)
  .AddScheme<AuthenticationSchemeOptions, ClientCredentialsAuthentication>(ClientCredentialsAuthentication.SchemeName, null)
  .AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidIssuer = jwtIssuer,
      ValidAudience = jwtAudience,
      ClockSkew = TimeSpan.Zero,
    });

builder.Services
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
  });

var app = builder.Build();

app.UseMiddleware<ErrorMiddleware>();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/no-auth", () => new SuccessResponse("You are in!"));

app
  .MapGet("/basic", (HttpContext context) => 
  {
    var user = context.User.Identity?.Name;
    return new SuccessResponse($"Hello, {user}!");
  })
  .RequireAuthorization(BasicAuthentication.SchemeName);

app
  .MapGet("/api-key", (HttpContext context) => 
  {
    var user = context.User.Identity?.Name;
    return new SuccessResponse($"Hello, {user}!");
  })
  .RequireAuthorization(ApiKeyAuthentication.SchemeName);

app
  .MapGet("/bearer", (HttpContext context) => 
  {
    var user = context.User.Identity?.Name;
    return new SuccessResponse($"Hello, {user}!");
  })
  .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);

app
  .MapPost("/generate-token", ([FromBody] LoginRequest loginRequest, HttpContext context) => 
  {
    if (loginRequest.IsValid is false)
    {
      return Results.ValidationProblem(new Dictionary<string, string[]>()
      {
        { "login", ["Invalid login request. Username and password are required."] }
      });
    }

    if (loginRequest.Username != "admin" || loginRequest.Password != "admin")
    {
      return Results.Problem(
        detail: "Invalid username or password.", 
        statusCode: StatusCodes.Status401Unauthorized
      );
    }

    var token = GenerateJwtToken(loginRequest.Username);
    var user = context.User.Identity?.Name;
    return Results.Ok(new { token = token.Value });
  });

app
  .MapPost("/access-token", (HttpContext context) => 
  {
    var token = GenerateJwtToken(context.User.Identity!.Name!);
    
    return Results.Ok(new { 
      token_type = "Bearer", 
      access_token = token.Value,
      expires_in = token.ExpiresInSecs
    });
  })
  .RequireAuthorization(ClientCredentialsAuthentication.SchemeName);

app.UseHttpsRedirection();

app.Run();

static (int ExpiresInSecs, string Value) GenerateJwtToken(string username)
{
  var tokenHandler = new JwtSecurityTokenHandler();
  var key = Encoding.ASCII.GetBytes(jwtSecret);
  var tokenDescriptor = new SecurityTokenDescriptor
  {
    Subject = new ClaimsIdentity([
      new Claim(ClaimTypes.NameIdentifier, username),
      new Claim(ClaimTypes.Name, username),
    ]),
    Expires = DateTime.UtcNow.AddDays(1),
    SigningCredentials = new SigningCredentials(
      new SymmetricSecurityKey(key), 
      SecurityAlgorithms.HmacSha256Signature
    ),
    Audience = jwtAudience,
    Issuer = jwtIssuer,
  };

  var token = tokenHandler.CreateToken(tokenDescriptor);
  return ((int)tokenDescriptor.Expires!.Value.Subtract(DateTime.UtcNow).TotalSeconds, tokenHandler.WriteToken(token));
}

/// <summary>
/// Partial class declaration for the Program class.
/// Allows using the Program class in integration tests.
/// </summary>
public partial class Program { }

record SuccessResponse(string Message);

record LoginRequest(string Username, string Password)
{
  public bool IsValid => string.IsNullOrWhiteSpace(Username) is false && string.IsNullOrWhiteSpace(Password) is false;
}

record ClientRequest : Bind
{
  public string ClientId { get; init; } = string.Empty;
  public string ClientSecret { get; init; } = string.Empty;
  public bool IsValid => string.IsNullOrWhiteSpace(ClientId) is false && string.IsNullOrWhiteSpace(ClientSecret) is false;

  public static ValueTask<ClientRequest> BindAsync(HttpContext context, ParameterInfo parameter)
  {
    throw new NotImplementedException();
  };
}