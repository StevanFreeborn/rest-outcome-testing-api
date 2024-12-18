using Bogus;

namespace API.FakeData;

class User
{
  public string Id { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;

  private static readonly Faker<User> Faker = new Faker<User>()
    .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
    .RuleFor(u => u.LastName, f => f.Name.LastName())
    .RuleFor(u => u.Email, f => f.Internet.Email())
    .RuleFor(u => u.Status, f => f.PickRandom("Active", "Inactive"));

  public static User Generate()
  {
    return Faker.Generate();
  }

  public static IEnumerable<User> Generate(int count)
  {
    return Faker.Generate(count);
  }
}