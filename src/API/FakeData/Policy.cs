using Bogus;

namespace API.FakeData;

class Policy
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

  private static readonly Faker<Policy> Faker = new Faker<Policy>()
    .RuleFor(p => p.Id, f => f.Random.Guid().ToString())
    .RuleFor(p => p.Name, f => f.PickRandom(PolicyNames.Names))
    .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
    .RuleFor(p => p.Status, f => f.PickRandom("Active", "Inactive"))
    .RuleFor(p => p.PublishedAt, f => f.Date.Past().Date.ToUniversalTime());

  public static Policy Generate()
  {
    return Faker.Generate();
  }

  public static IEnumerable<Policy> Generate(int count)
  {
    return Faker.Generate(count);
  }
}

static class PolicyNames
{
  public static string[] Names = [
    "Acceptable Use Policy",
    "Access Control Policy",
    "Data Retention Policy",
    "Data Privacy Policy",
    "Information Security Policy",
    "Password Management Policy",
    "Email Usage Policy",
    "Incident Response Policy",
    "Mobile Device Management Policy",
    "Remote Work Policy",
    "Third-Party Vendor Policy",
    "Cloud Security Policy",
    "Bring Your Own Device (BYOD) Policy",
    "Physical Security Policy",
    "Network Security Policy",
    "Disaster Recovery Policy",
    "Risk Assessment Policy",
    "Audit and Logging Policy",
    "Change Management Policy",
    "Encryption Policy",
    "Data Backup Policy",
    "Employee Code of Conduct Policy",
    "Security Awareness Training Policy",
    "Cybersecurity Policy",
    "Data Classification Policy",
    "Endpoint Security Policy",
    "GDPR Compliance Policy",
    "HIPAA Compliance Policy",
    "PCI DSS Compliance Policy",
    "Social Media Usage Policy",
    "Acceptable Encryption Standards Policy",
    "Vulnerability Management Policy",
    "Anti-Phishing Policy",
    "Insider Threat Policy",
    "Acceptable Monitoring Policy",
    "Asset Management Policy",
    "Software Development Lifecycle (SDLC) Policy",
    "Secure Software Development Policy",
    "Patch Management Policy",
    "Data Loss Prevention Policy",
    "Physical Media Disposal Policy",
    "Employee Termination Checklist Policy",
    "Firewall Configuration Policy",
    "Intrusion Detection and Prevention Policy",
    "Virtual Machine Security Policy",
    "Compliance Audit Policy",
    "Health and Safety Policy",
    "Internet Usage Policy",
    "Intellectual Property Policy",
    "Workplace Harassment Policy"
  ];
}