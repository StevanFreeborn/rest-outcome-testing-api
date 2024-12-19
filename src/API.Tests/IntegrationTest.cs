using System.Net.Http.Headers;

namespace API.Tests;

public class IntegrationTest(AppFactory appFactory) : IClassFixture<AppFactory>
{
  protected readonly HttpClient _client = appFactory.CreateClient();

  protected AuthenticationHeaderValue CreateBasicAuthHeader(string username, string password)
  {
    var credentials = $"{username}:{password}";
    var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
    return new("Basic", encodedCredentials);
  }
}