namespace Talon.Orleans.HelloWorld.Common;

/// <summary>
/// Represents a Chat Channel where users can join and
/// chat. Is more like Discord in that if you join you
/// can see the history of messages.
/// </summary>
public interface IChannelGrain : IGrainWithStringKey
{
  /// <summary>A client/user joins the channel and returns the approx Date/Time the connection will expire.</summary>
  Task<DateTime> JoinAsync(IChatClient client);

  /// <summary>A client/user leaves the channel.</summary>
  Task LeaveAsync(IChatClient client);

  /// <summary>A client/user posts a message to the channel.</summary>
  Task SendMessageAsync(string message, string userID);

  /// <summary>
  /// Returns a list of all the connected clients. Where each client 
  /// returns a Tuple, where Item1 is the Grain PrimaryKeyID and the
  /// userID.
  /// </summary>
  Task<List<Tuple<string,string>>> GetAllConnectedClientsAsync();
}