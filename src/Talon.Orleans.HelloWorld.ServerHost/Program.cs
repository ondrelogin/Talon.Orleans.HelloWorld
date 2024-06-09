namespace Talon.Orleans.HelloWorld.ServerHost;

internal static class Program
{
  /// <remarks>Main entry point of the web server</remarks>
  static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureHelloWorldServerHostServices();

    builder.Host.UseOrleans((hostBuilder, siloBuilder) =>
    {
      siloBuilder
        .UseLocalhostClustering()
        .ConfigureLogging(logging => logging.AddConsole());
    });

    using var host = builder.Build();

    host
      .UseDefaultFiles()
      .UseStaticFiles();
    host.MapControllers();

    await host.RunAsync();
  }

  /// <summary>performs all necessary dependency injection.</summary>
  private static IHostBuilder ConfigureHelloWorldServerHostServices(this IHostBuilder builder)
  {
    return builder.ConfigureServices(svcs =>
    {
      svcs.AddControllers();
    });
  }
}