namespace API.Tests;

public class DateTests(AppFactory appFactory) : IntegrationTest(appFactory)
{
  [Fact]
  public async Task DateBody_WhenDateValueIsSentInBody_ItShouldRespondWithProvidedValue()
  {
    var expected = DateTime.UtcNow.ToString();

    var response = await _client.PostAsJsonAsync("/date-body", new { dateValue = expected });

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();

    body.Should().Be(@$"{{""value"":""{expected}""}}");
  }

  [Fact]
  public async Task Date_WhenDateValueSentInUrl_ItShouldRespondWithProvidedValue()
  {
    var expected = DateTime.UtcNow.ToString();
    
    var response = await _client.GetAsync($"/date?date={expected}");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var body = await response.Content.ReadAsStringAsync();

    body.Should().Be(@$"{{""value"":""{expected}""}}");
  }
}