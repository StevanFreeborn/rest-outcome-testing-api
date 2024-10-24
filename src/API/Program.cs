
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

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
  .MapGet("/oauth2", (HttpContext context) => 
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

    var (_, value) = GenerateJwtToken(loginRequest.Username);
    return Results.Ok(new { token = value });
  });

app
  .MapPost("/access-token", (HttpContext context) => 
  {
    var (expiresInSecs, value) = GenerateJwtToken(context.User.Identity!.Name!);
    
    return Results.Ok(new { 
      token_type = "Bearer", 
      access_token = value,
      expires_in = expiresInSecs
    });
  })
  .RequireAuthorization(ClientCredentialsAuthentication.SchemeName);

app.MapPost("/json-files", async ([FromBody] FilesRequest request) => 
{
  await SaveFiles(request.Attachments);
  await SaveFiles(request.Images);

  return new SuccessResponse("Files received!");
});

app.MapPost("/date-body", ([FromBody] DateRequest request) => new { value = request.DateValue });

app.MapGet("/date", ([FromQuery] string date) => new { value = date });

app.MapPost("/list-value", ([FromBody] ListRequest request) => new { value = request.Value });

app.MapGet("/list/{value}", ([FromRoute] string value) => new { value });

app.MapGet("/simple-array", () => new[] { "one", "two", "three" } );

app.MapGet("/complex-object", () => new { value = new { name = "John", age = 30, hobbies = new[] { "reading", "swimming" } } });

app.MapGet("/complex-array", () => new[] { new { name = "John", age = 30 }, new { name = "Jane", age = 25 } });

app.MapGet("/users", () => new { value = "1" });

// Type A, 5bba9955-424e-49f9-ac36-d51b0417e71f
// Type B, 8ecbcf0b-3fa3-4cde-80f2-c2986d9aad4c
app.MapGet("/categories", () => new { value = "5bba9955-424e-49f9-ac36-d51b0417e71f" });

var record = new { 
  textField = "Text field value", 
  numberField = 123, 
  dateField = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
  timespanField = "1 Second(s)"
};

app.MapGet("/data", () => record);

app.MapGet("/timeout", async () => 
{
  await Task.Delay(11000);
  return new SuccessResponse("Timeout completed!");
});

var retryCount = 0;

app.MapGet("/retry", () => 
{
  if (retryCount < 4)
  {
    retryCount++;
    return Results.Problem("Retry failed!", statusCode: StatusCodes.Status404NotFound);
  }

  return Results.Ok(new SuccessResponse("Finally!"));
});

app.Run();

static async Task SaveFiles(string filesString)
{
  var files = filesString.Split(',');
  
  foreach (var file in files)
  {
    var fileBytes = Convert.FromBase64String(file);
    var fileName = Path.GetRandomFileName();
    var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "files");
    var filePath = Path.Combine(fileDirectory, fileName);

    if (Directory.Exists(fileDirectory) is false)
    {
      Directory.CreateDirectory(fileDirectory);
    }

    await File.WriteAllBytesAsync(filePath, fileBytes);

    Console.WriteLine($"File saved: {fileName}");
  }
}

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

record DateRequest(string DateValue);

record ListRequest(string Value);

record FilesRequest(string Attachments, string Images);