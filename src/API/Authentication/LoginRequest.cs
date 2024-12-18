namespace API.Authentication;

record LoginRequest(string Username, string Password)
{
  public bool IsValid() => string.IsNullOrWhiteSpace(Username) is false && string.IsNullOrWhiteSpace(Password) is false;
}