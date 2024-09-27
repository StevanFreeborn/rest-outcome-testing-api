
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
  .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(BasicAuthentication.SchemeName, null)
  .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthentication>(ApiKeyAuthentication.SchemeName, null);

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
  });

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

    var user = context.User.Identity?.Name;
    return Results.Ok();
  });

app.UseHttpsRedirection();

app.Run();

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