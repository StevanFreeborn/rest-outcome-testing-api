using API.Common;

namespace API.Timeout;

static class Extensions
{
  public static RouteGroupBuilder MapTimeoutEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/timeout").WithTags("Timeout");

    group
      .MapGet("/timeout", async () =>
      {
        await Task.Delay(31000);
        return Results.Ok(new SuccessResponse("Timeout completed!"));
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithTags("Timeout")
      .WithName("Timeout")
      .WithSummary("Timeout")
      .WithDescription("Simulates a timeout scenario.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized);

    return group;
  }
}