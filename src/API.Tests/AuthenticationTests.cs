using System.Net.Http.Headers;
using System.Text.Json.Nodes;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests;

public class AuthenticationTests(AppFactory appFactory) : IClassFixture<AppFactory>
{
  private readonly HttpClient _client = appFactory.CreateClient();

  private AuthenticationHeaderValue CreateBasicAuthHeader(string username, string password)
  {
    var credentials = $"{username}:{password}";
    var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
    return new("Basic", encodedCredentials);
  }

  [Fact]
  public async Task NoAuthEndpoint_WhenCalled_ItShouldReturnOk()
  {
    var response = await _client.GetAsync("/no-auth");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();
    body.Should().Be("""{"message":"You are in!"}""");
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithNoAuthHeader_ItShouldReturnUnauthorized()
  {
    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithWrongAuthorizationHeader_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = new("Bearer", "invalid");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithNoCredentials_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = new("Basic", "");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithNoPassword_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader("admin", "");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithNoUsername_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader("", "password");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithInvalidCredentials_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader("invalid", "invalid");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BasicAuthEndpoint_WhenCalledWithValidCredentials_ItShouldReturnOk()
  {
    _client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader("admin", "password");

    var response = await _client.GetAsync("/basic");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();
    body.Should().Be("""{"message":"Hello, admin!"}""");
  }

  [Fact]
  public async Task ApiKeyAuthEndpoint_WhenCalledWithNoApiKeyHeader_ItShouldReturnUnauthorized()
  {
    var response = await _client.GetAsync("/api-key");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task ApiKeyAuthEndpoint_WhenCalledWithWrongApiKeyHeader_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Add("x-api-key", "invalid");

    var response = await _client.GetAsync("/api-key");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task ApiKeyAuthEndpoint_WhenCalledWithValidApiKeyHeader_ItShouldReturnOk()
  {
    _client.DefaultRequestHeaders.Add("x-api-key", "api-key");

    var response = await _client.GetAsync("/api-key");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();
    body.Should().Be("""{"message":"Hello, API User!"}""");
  }

  [Fact]
  public async Task GenerateTokenEndpoint_WhenCalledWithNoLogin_ItShouldReturnBadRequest()
  {
    var response = await _client.PostAsJsonAsync("/generate-token", new { });

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task GenerateTokenEndpoint_WhenCalledWithInvalidLogin_ItShouldReturnUnauthorized()
  {
    var response = await _client.PostAsJsonAsync("/generate-token", new { username = "invalid", password = "invalid" });

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task GenerateTokenEndpoint_WhenCalledWithValidLogin_ItShouldReturnOk()
  {
    var response = await _client.PostAsJsonAsync("/generate-token", new { username = "admin", password = "admin" });

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadFromJsonAsync<JsonObject>();
    body.Should().ContainKey("token");
  }

  [Fact]
  public async Task BearerAuthEndpoint_WhenCalledWithNoAuthHeader_ItShouldReturnUnauthorized()
  {
    var response = await _client.GetAsync("/bearer");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BearerAuthEndpoint_WhenCalledWithWrongAuthorizationHeader_ItShouldReturnUnauthorized()
  {
    _client.DefaultRequestHeaders.Authorization = new("Bearer", "invalid");

    var response = await _client.GetAsync("/bearer");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task BearerAuthEndpoint_WhenCalledWithValidToken_ItShouldReturnOk()
  {
    var tokenResponse = await _client.PostAsJsonAsync("/generate-token", new { username = "admin", password = "admin" });
    var tokenBody = await tokenResponse.Content.ReadFromJsonAsync<JsonObject>();
    var token = tokenBody!["token"]!.ToString();

    _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

    var response = await _client.GetAsync("/bearer");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();
    body.Should().Be("""{"message":"Hello, admin!"}""");
  }

  [Fact]
  public async Task AccessTokenEndpoint_WhenCalledWithNoClient_ItShouldReturnBadRequest()
  {
    var response = await _client.PostAsJsonAsync("/access-token", new { });

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task AccessTokenEndpoint_WhenCalledWithInvalidClient_ItShouldReturnUnauthorized()
  {
    var response = await _client.PostAsJsonAsync("/access-token", new { client_id = "invalid", client_secret = "invalid" });

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task AccessTokenEndpoint_WhenCalledWithValidClientAsJson_ItShouldReturnOk()
  {
    var response = await _client.PostAsJsonAsync("/access-token", new { client_id = "client", client_secret = "secret" });

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadFromJsonAsync<JsonObject>();
    body.Should().ContainKey("token_type");
    body.Should().ContainKey("access_token");
    body.Should().ContainKey("expires_in");
  }

  [Fact]
  public async Task AccessTokenEndpoint_WhenCalledWithValidClientAsForm_ItShouldReturnOk()
  {
    var response = await _client.PostAsync("/access-token", new FormUrlEncodedContent(new Dictionary<string, string>
    {
      { "client_id", "client" },
      { "client_secret", "secret" }
    }));

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadFromJsonAsync<JsonObject>();
    body.Should().ContainKey("token_type");
    body.Should().ContainKey("access_token");
    body.Should().ContainKey("expires_in");
  }
}