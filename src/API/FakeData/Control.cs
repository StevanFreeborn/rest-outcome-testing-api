using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Bogus;

namespace API.FakeData;

record Control
{
  [Description("The unique identifier of the control")]
  [Required]
  public string Id { get; init; } = string.Empty;

  [Description("The name of the control")]
  [Required]
  public string Name { get; init; } = string.Empty;

  [Description("The unique identifier of the policy associated with the control")]
  [Required]
  public string PolicyId { get; init; } = string.Empty;

  [Description("The status of the control")]
  [Required]
  public string Status { get; init; } = string.Empty;

  private static readonly Faker<Control> Faker = new Faker<Control>()
    .RuleFor(c => c.Id, f => f.Random.Guid().ToString())
    .RuleFor(c => c.Name, f => f.PickRandom(ControlNames.Names))
    .RuleFor(c => c.PolicyId, f => f.Random.Guid().ToString())
    .RuleFor(c => c.Status, f => f.PickRandom("Not Tested", "Passed", "Failed"));

  public static Control Generate()
  {
    return Faker.Generate();
  }

  public static IEnumerable<Control> Generate(int count)
  {
    return Faker.Generate(count);
  }
}

static class ControlNames
{
  public static string[] Names = [
    "Access Control Enforcement",
    "Password Complexity Requirements",
    "Multi-Factor Authentication",
    "Role-Based Access Control",
    "Data Encryption in Transit",
    "Data Encryption at Rest",
    "Network Firewall Configuration",
    "Intrusion Detection System",
    "Security Information and Event Management (SIEM)",
    "Endpoint Protection",
    "Patch Management Process",
    "Backup and Recovery Plan",
    "Secure Software Development Lifecycle (SDLC)",
    "Vulnerability Scanning",
    "Incident Response Plan",
    "Data Loss Prevention (DLP)",
    "Identity and Access Management (IAM)",
    "Physical Access Restrictions",
    "Network Segmentation",
    "Mobile Device Management (MDM)",
    "Acceptable Use Monitoring",
    "Third-Party Risk Assessment",
    "Vendor Access Management",
    "Secure Remote Access",
    "Audit Log Review",
    "Regular Security Awareness Training",
    "Privileged Account Management",
    "Key Management Procedure",
    "Email Filtering and Anti-Spam",
    "Cloud Security Configurations",
    "Malware Protection",
    "Baseline Configuration Standards",
    "Application Whitelisting",
    "Secure API Gateway",
    "Data Classification Framework",
    "Change Management Process",
    "Business Continuity Plan",
    "Disaster Recovery Testing",
    "Sensitive Data Masking",
    "Unauthorized Device Detection",
    "IoT Security Standards",
    "Secure File Transfer Protocol (SFTP)",
    "Intrusion Prevention System",
    "Time-Based Access Control",
    "Dynamic Application Security Testing (DAST)",
    "Static Application Security Testing (SAST)",
    "Tokenization of Sensitive Data",
    "Continuous Monitoring Controls",
    "Zero Trust Network Architecture",
    "Remote Session Recording"
  ];
}