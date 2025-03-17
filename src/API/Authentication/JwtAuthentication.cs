using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

namespace API.Authentication;

class JwtAuthenticator
{
  public const string Secret = "This is a secret key for JWT token generation.";
  public const string Issuer = "TestingAPI";
  public const string Audience = "Onspring";

  public (int ExpiresInSecs, string Value) GenerateJwtToken(string username)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(Secret);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity([
        new Claim(ClaimTypes.NameIdentifier, username),
        new Claim(ClaimTypes.Name, username),
      ]),
      Expires = DateTime.UtcNow.AddDays(1),
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature
      ),
      Audience = Audience,
      Issuer = Issuer,
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return ((int)tokenDescriptor.Expires!.Value.Subtract(DateTime.UtcNow).TotalSeconds, tokenHandler.WriteToken(token));
  }
}