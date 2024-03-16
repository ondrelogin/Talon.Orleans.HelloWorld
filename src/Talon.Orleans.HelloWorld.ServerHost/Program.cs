namespace Talon.Orleans.HelloWorld.ServerHost;

internal static class Program
{
  static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

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
    
    await host.RunAsync();
  }
}