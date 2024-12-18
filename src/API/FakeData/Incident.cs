using Bogus;

namespace API.FakeData;

class Incident
{
  public string Id { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public string Priority { get; set; } = string.Empty;

  private static readonly Faker<Incident> Faker = new Faker<Incident>()
    .RuleFor(i => i.Id, f => f.Random.Guid().ToString())
    .RuleFor(i => i.Title, f => f.PickRandom(IncidentTitles.Titles))
    .RuleFor(i => i.Status, f => f.PickRandom("Open", "Closed", "In Progress"))
    .RuleFor(i => i.Priority, f => f.PickRandom("Low", "Medium", "High"));

  public static Incident Generate()
  {
    return Faker.Generate();
  }

  public static IEnumerable<Incident> Generate(int count)
  {
    return Faker.Generate(count);
  }
}

static class IncidentTitles
{
  public static string[] Titles = [
    "Phishing Email Reported",
    "Unauthorized Access Attempt",
    "Malware Detected",
    "Ransomware Attack",
    "Data Breach Discovered",
    "System Outage",
    "Insider Data Leak",
    "Suspicious Network Activity",
    "Compromised User Account",
    "Failed Login Attempt Spike",
    "Lost or Stolen Device",
    "Unauthorized Application Installation",
    "Policy Violation Detected",
    "Denial of Service (DoS) Attack",
    "Unapproved Data Transfer",
    "Misconfigured Firewall",
    "Social Engineering Incident",
    "Unpatched Software Exploited",
    "Suspicious Email Attachment",
    "Unauthorized USB Device Detected",
    "IoT Device Compromise",
    "Failed Backup Recovery",
    "Abnormal Data Download",
    "Physical Security Breach",
    "Third-Party Service Outage",
    "Critical System Failure",
    "Employee Harassment Report",
    "Credential Harvesting Attempt",
    "Privilege Escalation Attempt",
    "Sensitive Information Shared",
    "Data Loss During Migration",
    "Suspicious Cloud Access",
    "Fraudulent Transaction",
    "Misuse of Administrative Privileges",
    "Unauthorized Database Query",
    "Unsecured API Exploited",
    "Cross-Site Scripting (XSS) Detected",
    "SQL Injection Attempt",
    "Sensitive Data Exposed Online",
    "Key Management Failure",
    "Network Traffic Anomaly",
    "Physical Media Disposal Error",
    "Unauthorized Configuration Change",
    "Excessive Privilege Use",
    "Application Vulnerability Exploited",
    "Zero-Day Vulnerability Detected",
    "Suspicious Remote Access",
    "Unapproved Vendor Access",
    "Audit Log Tampering",
    "Abnormal Resource Utilization"
  ];
}