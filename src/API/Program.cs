using API.FakeData;
using API.OpenAPI;
using API.Retry;
using API.Timeout;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(o =>
{
  o.AddDocumentTransformer((document, context, cancellationToken) =>
  {
    document.Info = new()
    {
      Title = "REST API Outcome Playground",
      Version = "v1",
      Description = "A playground that can be used to explore and test REST API outcomes.",
    };
    return Task.CompletedTask;
  });
  o.AddDocumentTransformer<AuthDocumentTransformer>();
  o.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddSingleton<RetryTracker>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthN();
builder.Services.AddAuthZ();

var app = builder.Build();

app.UseMiddleware<ErrorMiddleware>();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapOpenApi();
app.MapScalarApiReference(o =>
{
  o.WithTitle("REST API Outcome Playground");
  o.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
  o.WithTheme(ScalarTheme.Purple);
  o.WithClientButton(false);
  o.WithFavicon("/favicon.ico");
});

app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.MapAuthEndpoints();

app.MapTimeoutEndpoints();

app.MapRetryEndpoints();

app.MapUserEndpoints();

app.MapPolicyEndpoints();

app.MapRisksEndpoints();

app.MapIncidentEndpoints();

app.MapControlEndpoints();

app.Run();

/// <summary>
/// Partial class declaration for the Program class.
/// Allows using the Program class in integration tests.
/// </summary>
public partial class Program { }