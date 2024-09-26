namespace API.Middleware;

class ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
{
  private readonly RequestDelegate _next = next;
  private readonly ILogger<ErrorMiddleware> _logger = logger;

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while processing the request");

      var problem = new ProblemDetails
      {
        Status = (int)HttpStatusCode.InternalServerError,
        Title = "Server error",
        Detail = "An problem occurred while processing the request",
      };

      context.Response.StatusCode = problem.Status.Value;
      context.Response.ContentType = "application/problem+json";
      await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
  }
}