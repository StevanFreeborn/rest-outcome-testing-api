
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
  .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services
  .AddAuthorizationBuilder()
  .AddPolicy("BasicAuthentication", policy =>
  {
    policy.RequireAuthenticatedUser();
    policy.RequireClaim(ClaimTypes.NameIdentifier);
  });

var app = builder.Build();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/noauth", () => new SuccessResponse("You are in!"));

app
  .MapGet("/basic", (HttpContext context) => 
  {
    var user = context.User.Identity?.Name;
    return new SuccessResponse($"Hello, {user}!");
  })
  .RequireAuthorization("BasicAuthentication");

app.UseHttpsRedirection();

app.Run();

/// <summary>
/// Partial class declaration for the Program class.
/// Allows using the Program class in integration tests.
/// </summary>
public partial class Program { }

record SuccessResponse(string Message);