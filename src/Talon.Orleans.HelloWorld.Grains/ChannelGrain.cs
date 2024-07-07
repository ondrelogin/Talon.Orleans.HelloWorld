using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using Talon.Orleans.HelloWorld.Common;
using Talon.Orleans.HelloWorld.Common.Messages;

namespace Talon.Orleans.HelloWorld.Grains;

/// <inheritdoc />
public class ChannelGrain : Grain, IChannelGrain
{
  private readonly ILogger _logger;
  private readonly ObserverManager<IChatClient> _clients;

  public ChannelGrain(ILogger<ChannelGrain> logger)
  {
    _logger = logger;
    _clients = new ObserverManager<IChatClient>(TimeSpan.FromMinutes(5), logger);
  }

  /// <inheritdoc />
  public async Task<DateTime> JoinAsync(IChatClient client)
  {
    string userID = await client.GetUserID();
    _logger.LogDebug($"{userID} client joining...");

    bool userExist = await this.DoesUserExistAsync(userID);

    var now = _clients.GetDateTime();
    var expireDateTimeUtc = now.Add(_clients.ExpirationDuration);

    _clients.Subscribe(client, client);

    if (!userExist)
    {
      var msg = new UserEventMessage(userID, "Join", DateTime.UtcNow);
      await _clients.Notify(n => n.OnEventReceivedAsync(msg));
    }    

    _logger.LogInformation($"{userID} subscribed until {expireDateTimeUtc:HH:mm:ss}");
    return expireDateTimeUtc;
  }

  /// <inheritdoc />
  public async Task LeaveAsync(IChatClient client)
  {
    string userID = "??";
    try
    {
      userID = await client.GetUserID();
    }
    catch { }

    _logger.LogDebug($"{userID} client leaving...");
    _clients.Unsubscribe(client);

    var msg = new UserEventMessage(userID, "Leave", DateTime.UtcNow);
    await _clients.Notify(n => n.OnEventReceivedAsync(msg));
  }

  /// <inheritdoc />
  public async Task SendMessageAsync(string message, string playerID)
  {
    var msg = new ChatMessage(playerID, message, DateTime.UtcNow);
    // TODO add msg to history of messages.

    // intentionally sending to all clients, probable that the client that sent this message will receive
    //   it? will just need to filter as appropriate in the client logic?
    await _clients.Notify(n => n.OnMessageReceivedAsync(msg));
  }

  /// <inheritdoc />
  public async Task<List<Tuple<string, string>>> GetAllConnectedClientsAsync()
  {
    var list = new List<Tuple<string, string>>();
    foreach (var client in _clients)
    {
      string playerID = await client.GetUserID();

      list.Add(Tuple.Create(client.GetPrimaryKeyString(), playerID));
    }
    return list;
  }

  /// <summary>
  /// If the userId is in the clients will return true. Due
  /// to the async nature of this perhaps there is a better
  /// way of doing this, but this project is not for fully optimized
  /// code but simple and easy to understandable code, so I will
  /// skip any potential optimization here.
  /// </summary>
  private async Task<bool> DoesUserExistAsync(string userID)
  {
    foreach (var c in _clients)
    {
      string clientUserID = await c.GetUserID();
      if (clientUserID == userID) return true;
    }
    return false;
  }
}