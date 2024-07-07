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
        .ConfigureServices(RegisterDi)
        .ConfigureLogging(logging => logging.ClearProviders())
        .UseConsoleLifetime();

    using IHost host = builder.Build();

    // init view
    ConsoleView view = host.Services.GetRequiredService<ConsoleView>();
    var userID = view.PromptForUserID();
    if (userID == null) return;

    // start host
    await host.StartAsync();

    // connect
    view.Status("Connecting to server..");
    IClusterClient orleansClient = host.Services.GetRequiredService<IClusterClient>();
    var channel = orleansClient.GetGrain<IChannelGrain>("general");
    var channelEvents = new ChannelEventClient(view, userID);
    var channelRef = orleansClient.CreateObjectReference<IChatClient>(channelEvents);

    view.Status("Joining server..");
    var expireTimeUtc = await channel.JoinAsync(channelRef);
    var dur = expireTimeUtc.Subtract(DateTime.UtcNow);

    // launch watcher to handle reconnection of client...
    var watcher = host.Services.GetRequiredService<ChannelWatcherService>();
    await watcher.StartAsync(new CancellationTokenSource().Token);
    watcher.Init(view, dur, channel, channelRef, expireTimeUtc);
    await watcher.StartAsync(new CancellationTokenSource().Token);

    // loop user input until user types "exit"
    try
    {
      bool shouldExit = false;
      while (!shouldExit)
      {
        var textInput = view.GetUserInput();
        if ("exit".EqualsAnyCase(textInput) || "/exit".EqualsAnyCase(textInput))
        {
          shouldExit = true;
        }
        else if ("/now".EqualsAnyCase(textInput))
        {
          view.Status("now");
        }
        else if ("/status".EqualsAnyCase(textInput))
        {
          watcher.CheckExpireStatus();
        }
        else if ("/all".EqualsAnyCase(textInput))
        {
          var listUsers = await channel.GetAllConnectedClientsAsync();
          foreach (var u in listUsers)
          {
            view.Status($"- {u.Item1} : {u.Item2}");
          }
        }
        else if (string.IsNullOrWhiteSpace(textInput))
        {
          // do nothing
        }
        else
        {
          await channel.SendMessageAsync(textInput, userID);
        }
      }
    }
    catch (Exception ex)
    {
      view.Info("ERROR:");
      view.Info(ex.Message);
    }
    finally
    {
      view.Status("Leaving channel...");
      await channel.LeaveAsync(channelRef);
      channelEvents = null;
      channelRef = null;
    }

    // exiting
    view.Status("Closing...");
    var t1 = host.StopAsync();
    var t2 = watcher.StopAsync(new CancellationTokenSource().Token);
    await t1;
    await t2;
  }

  private static void RegisterDi(HostBuilderContext ctx, IServiceCollection services)
  {
    services.AddTransient<ConsoleView>();
    services.AddSingleton<ChannelWatcherService>();

    services.AddHostedService<ChannelWatcherService>();
  }
}
