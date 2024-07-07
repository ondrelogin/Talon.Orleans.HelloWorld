using Talon.Orleans.HelloWorld.Common.Messages;

namespace Talon.Orleans.HelloWorld.Common;

/// <summary>
/// The client observer interface to react to activity in
/// a given <see cref="IChannelGrain"/>
/// </summary>
public interface IChatClient : IGrainObserver
{
  /// <summary>Returns the UserID of the ChatClient.</summary>
  ValueTask<string> GetUserID();

  /// <summary>
  /// Sends a "real-time" message to the client.
  /// </summary>
  Task OnMessageReceivedAsync(ChatMessage message);

  /// <summary>
  /// The server notifies the client that a user event occurred.
  /// Typically a Join or Leave Channel message.
  /// </summary>
  Task OnEventReceivedAsync(UserEventMessage message);
}