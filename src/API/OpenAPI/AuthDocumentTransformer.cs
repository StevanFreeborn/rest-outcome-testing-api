using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace API.OpenAPI;

class AuthDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
  public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
  {
    document.Components ??= new OpenApiComponents();

    var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

    if (authenticationSchemes.Any(authScheme => authScheme.Name is JwtBearerDefaults.AuthenticationScheme))
    {
      var requirement = new OpenApiSecurityScheme()
      {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        BearerFormat = "Json Web Token",
      };

      document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, requirement);
    }

    if (authenticationSchemes.Any(authScheme => authScheme.Name is BasicAuthentication.SchemeName))
    {
      var requirement = new OpenApiSecurityScheme()
      {
        Type = SecuritySchemeType.Http,
        Scheme = BasicAuthentication.SchemeName,
        In = ParameterLocation.Header,
      };

      document.Components.SecuritySchemes.Add(BasicAuthentication.SchemeName, requirement);
    }

    if (authenticationSchemes.Any(authScheme => authScheme.Name is ApiKeyAuthentication.SchemeName))
    {
      var requirement = new OpenApiSecurityScheme()
      {
        Type = SecuritySchemeType.ApiKey,
        Scheme = ApiKeyAuthentication.SchemeName,
        Name = ApiKeyAuthentication.ApiKeyHeaderName,
        In = ParameterLocation.Header,
      };

      document.Components.SecuritySchemes.Add(ApiKeyAuthentication.SchemeName, requirement);
    }

    if (authenticationSchemes.Any(authScheme => authScheme.Name is ClientCredentialsAuthentication.SchemeName))
    {
      var requirement = new OpenApiSecurityScheme()
      {
        Type = SecuritySchemeType.OAuth2,
        Scheme = ClientCredentialsAuthentication.SchemeName,
        Flows = new OpenApiOAuthFlows
        {
          ClientCredentials = new OpenApiOAuthFlow
          {
            TokenUrl = new Uri("/access-token", UriKind.Relative),
          }
        }
      };

      document.Components.SecuritySchemes.Add(ClientCredentialsAuthentication.SchemeName, requirement);
    }
  }
}