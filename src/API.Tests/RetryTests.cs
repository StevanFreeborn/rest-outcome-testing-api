namespace API.Tests;

public class RetryTests(AppFactory appFactory) : IntegrationTest(appFactory)
{
  [Fact]
  public async Task Retry_WhenCalled_ItShouldReturnSuccessAfterThreeRequests()
  {
    var id = "test";
    var endpoint = $"/retry/{id}";

    _client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader("admin", "password");

    Task<HttpResponseMessage> MakeRequest() => _client.GetAsync(endpoint);

    var response = await MakeRequest();

    response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

    response = await MakeRequest();

    response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

    response = await MakeRequest();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}