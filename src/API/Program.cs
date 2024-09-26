
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

app.UseHttpsRedirection();

app.Run();

/// <summary>
/// Partial class declaration for the Program class.
/// Allows using the Program class in integration tests.
/// </summary>
public partial class Program { }

record SuccessResponse(string Message);