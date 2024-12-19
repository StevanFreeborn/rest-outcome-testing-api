
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace API.OpenAPI;

class AuthOperationTransformer : IOpenApiOperationTransformer
{
  private static readonly OpenApiSecurityRequirement JwtBearerSecurityScheme = new()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = JwtBearerDefaults.AuthenticationScheme,
        },
      },
      Array.Empty<string>()
    }
  };

  private static readonly OpenApiSecurityRequirement BasicAuthenticationSecurityScheme = new()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = BasicAuthentication.SchemeName,
        },
      },
      Array.Empty<string>()
    }
  };

  private static readonly OpenApiSecurityRequirement ApiKeyAuthenticationSecurityScheme = new()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = ApiKeyAuthentication.SchemeName,
        },
      },
      Array.Empty<string>()
    }
  };

  private static readonly OpenApiSecurityRequirement ClientCredentialsAuthenticationSecurityScheme = new()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = ClientCredentialsAuthentication.SchemeName,
        },
      },
      Array.Empty<string>()
    }
  };

  private readonly Dictionary<string, List<OpenApiSecurityRequirement>> _securityRequirements = new()
  {
    [JwtBearerDefaults.AuthenticationScheme] = [JwtBearerSecurityScheme],
    [BasicAuthentication.SchemeName] = [BasicAuthenticationSecurityScheme],
    [ApiKeyAuthentication.SchemeName] = [ApiKeyAuthenticationSecurityScheme],
    [ClientCredentialsAuthentication.SchemeName] = [ClientCredentialsAuthenticationSecurityScheme],
    [CombinedAuthentication.SchemeName] = [
      ClientCredentialsAuthenticationSecurityScheme,
      JwtBearerSecurityScheme,
      BasicAuthenticationSecurityScheme,
      ApiKeyAuthenticationSecurityScheme,
    ]
  };


  public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
  {
    var authAttributes = context.Description.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().ToList();

    foreach (var authAttribute in authAttributes)
    {
      if (authAttribute.Policy is not null && _securityRequirements.TryGetValue(authAttribute.Policy, out var securityRequirements))
      {
        foreach (var securityRequirement in securityRequirements)
        {
          operation.Security.Add(securityRequirement);
        }
      }
    }

    return Task.CompletedTask;
  }
}
