using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Bogus;

namespace API.FakeData;

record User
{
  [Description("The unique identifier of the user")]
  [Required]
  public string Id { get; init; } = string.Empty;

  [Description("The first name of the user")]
  [Required]
  public string FirstName { get; init; } = string.Empty;

  [Description("The last name of the user")]
  [Required]
  public string LastName { get; init; } = string.Empty;

  [Description("The email address of the user")]
  [Required]
  public string Email { get; init; } = string.Empty;

  [Description("The status of the user")]
  [Required]
  public string Status { get; init; } = string.Empty;

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