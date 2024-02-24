using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Talon.Orleans.HelloWorld.ServerHost;

internal static class Program
{
  static async Task Main(string[] args)
  {
    var builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(silo =>
    {
      silo
        .UseLocalhostClustering()
        .ConfigureLogging(logging => logging.AddConsole());
    })
    .UseConsoleLifetime();

    using IHost host = builder.Build();
    await host.RunAsync();
  }
}