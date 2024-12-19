using System.ComponentModel;

namespace API.FakeData;

static class Extensions
{
  public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/users").WithTags("Users");

    group
      .MapGet("", () =>
      {
        var users = User.Generate(10);
        return Results.Ok(users);
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetUsers")
      .WithSummary("Get Users")
      .WithDescription("Gets a list of users.")
      .Produces<IEnumerable<User>>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("/{id}", ([Description("The unique identifier of the user.")] string id) =>
      {
        var user = User.Generate();
        return Results.Ok(user with { Id = id });
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetUserById")
      .WithSummary("Get User By Id")
      .WithDescription("Gets a user by their unique identifier.")
      .Produces<User>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }

  public static RouteGroupBuilder MapPolicyEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/policies").WithTags("Policies");

    group
      .MapGet("", () =>
      {
        var policies = Policy.Generate(10);
        return Results.Ok(policies);
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithTags("Policies")
      .WithName("GetPolicies")
      .WithSummary("Get Policies")
      .WithDescription("Gets a list of policies.")
      .Produces<IEnumerable<Policy>>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("{id}", ([Description("The unique identifier of the policy.")] string id) =>
      {
        var policy = Policy.Generate();
        return Results.Ok(policy with { Id = id });
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithTags("Policies")
      .WithName("GetPolicyById")
      .WithSummary("Get Policy By Id")
      .WithDescription("Gets a policy by its unique identifier.")
      .Produces<Policy>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }

  public static RouteGroupBuilder MapRisksEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/risks").WithTags("Risks");

    group
      .MapGet("", () =>
      {
        var risks = Risk.Generate(10);
        return Results.Ok(risks);
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetRisks")
      .WithSummary("Get Risks")
      .WithDescription("Gets a list of risks.")
      .Produces<IEnumerable<Risk>>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("{id}", ([Description("The unique identifier of the risk.")] string id) =>
      {
        var risk = Risk.Generate();
        return Results.Ok(risk with { Id = id });
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetRiskById")
      .WithSummary("Get Risk By Id")
      .WithDescription("Gets a risk by its unique identifier.")
      .Produces<Risk>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }

  public static RouteGroupBuilder MapIncidentEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/incidents").WithTags("Incidents");

    group
      .MapGet("", () =>
      {
        var incidents = Incident.Generate(10);
        return Results.Ok(incidents);
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetIncidents")
      .WithSummary("Get Incidents")
      .WithDescription("Gets a list of incidents.")
      .Produces<IEnumerable<Incident>>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("{id}", ([Description("The unique identifier of the incident.")] string id) =>
      {
        var incident = Incident.Generate();
        return Results.Ok(incident with { Id = id });
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetIncidentById")
      .WithSummary("Get Incident By Id")
      .WithDescription("Gets an incident by its unique identifier.")
      .Produces<Incident>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }

  public static RouteGroupBuilder MapControlEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/controls").WithTags("Controls");

    group
      .MapGet("", () =>
      {
        var controls = Control.Generate(10);
        return Results.Ok(controls);
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetControls")
      .WithSummary("Get Controls")
      .WithDescription("Gets a list of controls.")
      .Produces<IEnumerable<Control>>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    group
      .MapGet("{id}", ([Description("The unique identifier of the control.")] string id) =>
      {
        var control = Control.Generate();
        return Results.Ok(control with { Id = id });
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithName("GetControlById")
      .WithSummary("Get Control By Id")
      .WithDescription("Gets a control by its unique identifier.")
      .Produces<Control>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }
}