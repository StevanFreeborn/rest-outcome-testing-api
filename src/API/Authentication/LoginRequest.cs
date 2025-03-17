using System.ComponentModel;

namespace API.Authentication;

record LoginRequest(
  [property: Description("The username of the user")]
  string Username,
  [property: Description("The password of the user")]
  string Password
)
{
  public bool IsValid() => string.IsNullOrWhiteSpace(Username) is false && string.IsNullOrWhiteSpace(Password) is false;
}