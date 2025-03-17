using API.FakeData;

namespace API.Tests;

public class FakeDataTests : IntegrationTest
{
  public FakeDataTests(AppFactory appFactory) : base(appFactory)
  {
    _client.DefaultRequestHeaders.Add("x-api-key", "api-key");
  }

  [Fact]
  public async Task GetUsers_WhenCalled_ItShouldReturnListOfUsers()
  {
    var response = await _client.GetAsync("/users");
    var users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    users.Should().HaveCountGreaterThan(0);
  }

  [Fact]
  public async Task GetUserById_WhenCalled_ItShouldReturnUser()
  {
    var response = await _client.GetAsync("/users/1");
    var user = await response.Content.ReadFromJsonAsync<User>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    user!.Id.Should().Be("1");
  }

  [Fact]
  public async Task GetPolicies_WhenCalled_ItShouldReturnListOfPolicies()
  {
    var response = await _client.GetAsync("/policies");
    var policies = await response.Content.ReadFromJsonAsync<IEnumerable<Policy>>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    policies.Should().HaveCountGreaterThan(0);
  }

  [Fact]
  public async Task GetPolicyById_WhenCalled_ItShouldReturnPolicy()
  {
    var response = await _client.GetAsync("/policies/1");
    var policy = await response.Content.ReadFromJsonAsync<Policy>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    policy!.Id.Should().Be("1");
  }

  [Fact]
  public async Task GetRisks_WhenCalled_ItShouldReturnListOfRisks()
  {
    var response = await _client.GetAsync("/risks");
    var risks = await response.Content.ReadFromJsonAsync<IEnumerable<Risk>>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    risks.Should().HaveCountGreaterThan(0);
  }

  [Fact]
  public async Task GetRiskById_WhenCalled_ItShouldReturnRisk()
  {
    var response = await _client.GetAsync("/risks/1");
    var risk = await response.Content.ReadFromJsonAsync<Risk>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    risk!.Id.Should().Be("1");
  }

  [Fact]
  public async Task GetIncidents_WhenCalled_ItShouldReturnListOfIncidents()
  {
    var response = await _client.GetAsync("/incidents");
    var incidents = await response.Content.ReadFromJsonAsync<IEnumerable<Incident>>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    incidents.Should().HaveCountGreaterThan(0);
  }

  [Fact]
  public async Task GetIncidentById_WhenCalled_ItShouldReturnIncident()
  {
    var response = await _client.GetAsync("/incidents/1");
    var incident = await response.Content.ReadFromJsonAsync<Incident>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    incident!.Id.Should().Be("1");
  }
}