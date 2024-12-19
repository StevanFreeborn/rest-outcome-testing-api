using System.ComponentModel;

using API.Common;

namespace API.Retry;

static class Extensions
{
  public static RouteGroupBuilder MapRetryEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/retry").WithTags("Retry");

    group
      .MapGet("{id}", (
        [Description("The unique identifier to track the retry count.")] string id,
        [FromServices] RetryTracker tracker
      ) =>
      {
        if (tracker.Counts.TryGetValue(id, out var retryCount) is false)
        {
          tracker.Counts[id] = 0;
          retryCount = 0;
        }


        if (retryCount < 2)
        {
          tracker.Counts[id] = retryCount + 1;
          return Results.Problem("Retry failed!", statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        tracker.Counts.Remove(id);

        return Results.Ok(new SuccessResponse("Finally!"));
      })
      .RequireAuthorization(CombinedAuthentication.SchemeName)
      .WithTags("Retry")
      .WithName("Retry")
      .WithSummary("Retry")
      .WithDescription("Simulates a retry scenario.")
      .Produces<SuccessResponse>((int)HttpStatusCode.OK)
      .Produces<ProblemDetails>((int)HttpStatusCode.Unauthorized)
      .Produces<ProblemDetails>((int)HttpStatusCode.ServiceUnavailable);

    return group;
  }
}