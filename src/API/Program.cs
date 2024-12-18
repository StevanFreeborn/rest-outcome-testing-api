
using API.Common;
using API.FakeData;
using API.OpenAPI;
using API.Retry;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(o => o.AddDocumentTransformer<AuthTransformer>());

builder.Services.AddSingleton<RetryTracker>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthN();
builder.Services.AddAuthZ();

var app = builder.Build();

app.UseMiddleware<ErrorMiddleware>();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();

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

    var (_, value) = jwtAuth.GenerateJwtToken(loginRequest.Username);
    return Results.Ok(new { token = value });
  });

app
  .MapPost("/access-token", (HttpContext context, [FromServices] JwtAuthenticator jwtAuth) => 
  {
    var (expiresInSecs, value) = jwtAuth.GenerateJwtToken(context.User.Identity!.Name!);
    
    return Results.Ok(new { 
      token_type = "Bearer", 
      access_token = value,
      expires_in = expiresInSecs
    });
  })
  .RequireAuthorization(ClientCredentialsAuthentication.SchemeName);

app
  .MapGet("/oauth2", (HttpContext context) => 
  {
    var user = context.User.Identity?.Name;
    return new SuccessResponse($"Hello, {user}!");
  })
  .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);

app.MapGet("/timeout", async () => 
{
  await Task.Delay(11000);
  return new SuccessResponse("Timeout completed!");
});

app
  .MapGet("/retry/{id}", (string id, [FromServices] RetryTracker tracker) => 
  {  
    if (tracker.Counts.TryGetValue(id, out var retryCount) is false)
    {
      tracker.Counts[id] = 0;
      retryCount = 0;
    }
    

    if (retryCount < 2)
    {
      tracker.Counts[id] = retryCount + 1;
      return Results.Problem("Retry failed!", statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    tracker.Counts.Remove(id);

    return Results.Ok(new SuccessResponse("Finally!"));
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/users", () => 
  {
    var users = User.Generate(10);
    return Results.Ok(users);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/users/{id}", (string id) => 
  {
    var user = User.Generate();
    user.Id = id;
    return Results.Ok(user);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/policies", () => 
  {
    var policies = Policy.Generate(10);
    return Results.Ok(policies);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/policies/{id}", (string id) => 
  {
    var policy = Policy.Generate();
    policy.Id = id;
    return Results.Ok(policy);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/risks", () => 
  {
    var risks = Risk.Generate(10);
    return Results.Ok(risks);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/risks/{id}", (string id) => 
  {
    var risk = Risk.Generate();
    risk.Id = id;
    return Results.Ok(risk);
})
.RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/incidents", () => 
  {
    var incidents = Incident.Generate(10);
    return Results.Ok(incidents);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/incidents/{id}", (string id) => 
  {
    var incident = Incident.Generate();
    incident.Id = id;
    return Results.Ok(incident);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/controls", () => 
  {
    var controls = Control.Generate(10);
    return Results.Ok(controls);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app
  .MapGet("/controls/{id}", (string id) => 
  {
    var control = Control.Generate();
    control.Id = id;
    return Results.Ok(control);
  })
  .RequireAuthorization(CombinedAuthentication.SchemeName);

app.Run();

/// <summary>
/// Partial class declaration for the Program class.
/// Allows using the Program class in integration tests.
/// </summary>
public partial class Program { }
