namespace API.Tests;

public class IntegrationTest(AppFactory appFactory) : IClassFixture<AppFactory>
{
  protected readonly HttpClient _client = appFactory.CreateClient();
}