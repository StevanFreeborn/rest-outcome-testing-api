
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Tests;

public class AppFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureLogging(logging => logging.ClearProviders());
  }
}