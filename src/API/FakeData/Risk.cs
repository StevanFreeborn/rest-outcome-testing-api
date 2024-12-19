using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Bogus;

namespace API.FakeData;

record Risk
{
  [Description("The unique identifier of the risk")]
  [Required]
  public string Id { get; init; } = string.Empty;

  [Description("The title of the risk")]
  [Required]
  public string Title { get; init; } = string.Empty;

  [Description("The severity of the risk")]
  [Required]
  public string Severity { get; init; } = string.Empty;

  [Description("The impact of the risk")]
  [Required]
  public string Impact { get; init; } = string.Empty;

  private static readonly Faker<Risk> Faker = new Faker<Risk>()
    .RuleFor(r => r.Id, f => f.Random.Guid().ToString())
    .RuleFor(r => r.Title, f => f.PickRandom(RiskTitles.Titles))
    .RuleFor(r => r.Severity, f => f.PickRandom(RiskRatings.Ratings))
    .RuleFor(r => r.Impact, f => f.PickRandom(RiskRatings.Ratings));

  public static Risk Generate()
  {
    return Faker.Generate();
  }

  public static IEnumerable<Risk> Generate(int count)
  {
    return Faker.Generate(count);
  }
}

static class RiskTitles
{
  public static string[] Titles = [
    "Data Breach",
    "Unauthorized Access",
    "Phishing Attack",
    "Malware Infection",
    "Ransomware Incident",
    "Insider Threat",
    "Vendor Non-Compliance",
    "Data Loss",
    "System Downtime",
    "Weak Passwords",
    "Social Engineering Attack",
    "Cloud Misconfiguration",
    "Unpatched Vulnerabilities",
    "Physical Security Breach",
    "Denial of Service (DoS) Attack",
    "Man-in-the-Middle Attack",
    "Compromised Credentials",
    "Loss of Physical Media",
    "Unsecured Wi-Fi Usage",
    "Insufficient Encryption",
    "Regulatory Non-Compliance",
    "Third-Party Data Breach",
    "Mobile Device Theft",
    "IoT Device Vulnerability",
    "Shadow IT Risk",
    "Lack of Security Training",
    "Overprivileged Users",
    "Software Supply Chain Attack",
    "Key Management Failure",
    "Privileged Account Misuse",
    "Insufficient Logging and Monitoring",
    "Critical System Failure",
    "Fraudulent Transactions",
    "Intellectual Property Theft",
    "Privacy Violation",
    "Excessive Permissions",
    "API Security Breach",
    "Lack of Backup",
    "Natural Disaster Impact",
    "Cyber Espionage",
    "Outdated Systems",
    "Policy Violation",
    "Email Spoofing",
    "Misuse of Sensitive Data",
    "Cross-Site Scripting (XSS)",
    "SQL Injection",
    "Supply Chain Disruption",
    "Inadequate Incident Response",
    "Lack of Governance",
    "Zero-Day Exploit"
  ];
}

static class RiskRatings
{
  public static string[] Ratings = [
    "Low",
    "Medium",
    "High",
    "Critical"
  ];
}