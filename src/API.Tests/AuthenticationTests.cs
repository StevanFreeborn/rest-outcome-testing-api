using System.Net.Http.Headers;

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
    var response = await _client.GetAsync("/noauth");

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
}