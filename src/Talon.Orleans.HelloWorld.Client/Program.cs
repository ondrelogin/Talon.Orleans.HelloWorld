using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Talon.Orleans.HelloWorld.Common;

namespace Talon.Orleans.HelloWorld.Client;

internal class Program
{
  static async Task Main(string[] args)
  {
    // init
    var builder = Host.CreateDefaultBuilder(args)
      .UseOrleansClient(client =>
      {
        client.UseLocalhostClustering();
      })
      .ConfigureLogging(logging => logging.AddConsole())
      .UseConsoleLifetime();

    using IHost host = builder.Build();
    await host.StartAsync();

    // connecting to server
    Console.WriteLine("connecting to grain server...");
    var grainFactory = host.Services.GetRequiredService<IClusterClient>();
    var bot = grainFactory.GetGrain<IBot>("num5");

    // prompt user for name
    Console.Write("enter your name:");
    string? name = Console.ReadLine();
    Console.WriteLine();
    if (string.IsNullOrWhiteSpace(name)) { name = "No name"; }
    Console.WriteLine();

    // getting response
    string response = await bot.SayHello(name);
    Console.WriteLine("{0}", response);
    Console.WriteLine();

    // exiting
    await host.StopAsync();
  }
}
