using System.ComponentModel;
using System.Text.Json.Serialization;

record LoginResponse(
  [property: Description("The type of token issued by the server")]
  [property: JsonPropertyName("token_type")]
  string TokenType,
  [property: Description("The access token issued by the server")]
  [property: JsonPropertyName("access_token")]
  string AccessToken,
  [property: Description("The refresh token issued by the server")]
  [property: JsonPropertyName("expires_in")]
  int ExpiresInSecs
);