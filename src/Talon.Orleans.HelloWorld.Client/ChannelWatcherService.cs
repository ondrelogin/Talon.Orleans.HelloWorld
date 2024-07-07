using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Talon.Orleans.HelloWorld.Common;

namespace Talon.Orleans.HelloWorld.Client;

/// <summary>
/// This service is in charge of periodically rejoining the server grain.
/// This is because the Grain architecture (ObserverManager) is designed to
/// disconnect clients periodically. I assume this is so the server can clean
/// up "crashed" clients that did not get a chance to disconnect gracefully.
/// But to maintain connectivity, the Client must reconnect occasionally. So
/// this class is here to maintain the connection.
/// </summary>
public class ChannelWatcherService : IHostedService, IDisposable
{
  private readonly ILogger<ChannelWatcherService> _logger;

  private Timer? _timer;
  private TimeSpan _interval;
  private IConsoleView? _view;

  private IChannelGrain? _channel;
  private IChatClient? _channelRef;
  private DateTime _expireDateTimeUtc;

  public ChannelWatcherService(ILogger<ChannelWatcherService> logger)
  {
    _logger = logger;

    _timer = null;
    _interval = TimeSpan.Zero;
    _view = null;

    _channel = null;
    _channelRef = null; 
    _expireDateTimeUtc = DateTime.UtcNow;
  }

  /// <summary>Passes in all the necessary parameters so the client can reconnect/rejoin.</summary>
  public void Init(ConsoleView view, TimeSpan dur, IChannelGrain channel, IChatClient channelRef, DateTime expireTimeUtc)
  {
    _view = view;
    _channel = channel; 
    _channelRef = channelRef;
    _expireDateTimeUtc = expireTimeUtc;

    double seconds = dur.TotalSeconds * 0.7;
    _interval = TimeSpan.FromSeconds(seconds);

    _view.Status($"- watcher interval is {_interval.TotalSeconds:N1} seconds..");
  }

  /// <inheritdoc />
  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Timed Hosted Service running.");

    _timer = new Timer(DoWork, null, TimeSpan.Zero, _interval);

    return Task.CompletedTask;
  }

  /// <summary>
  /// This class is in charge of maintaining the expiration of the client connection
  /// so this class is what 
  /// </summary>
  public void CheckExpireStatus()
  {
    if (_view != null)
    {
      _view.Status($"Expire is at {_expireDateTimeUtc:HH:mm:ss}..");
    }
  }

  /// <inheritdoc />
  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Timed Hosted Service is stopping.");
    _timer?.Change(Timeout.Infinite, 0);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public void Dispose()
  {
    var local = _timer;
    _timer = null;

    if (local == null) return;
    local.Dispose();
  }

  /// <summary>
  /// The actual call that gets called periodically that attempts 
  /// to rejoin the server/channel.
  /// </summary>
  private void DoWork(object? state)
  {
    if (_channel != null && _channelRef != null)
    {
      _expireDateTimeUtc = _channel.JoinAsync(_channelRef).Result;
    }
  }
}
