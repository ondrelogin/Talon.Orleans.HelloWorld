using Talon.Orleans.HelloWorld.Common;
using Talon.Orleans.HelloWorld.Common.Messages;

namespace Talon.Orleans.HelloWorld.Client;

/// <summary>
/// Handles the various incoming traffic from the Actor server
/// and responds accordingly.
/// </summary>
public class ChannelEventClient : IChatClient
{
  private readonly ConsoleView _view;
  private readonly string _userID;

  public ChannelEventClient(ConsoleView view, string userID)
  {
    _view = view;
    _userID = userID;
  }

  /// <inheritdoc />
  public ValueTask<string> GetUserID() { return ValueTask.FromResult(_userID); }

  /// <inheritdoc />
  public Task OnMessageReceivedAsync(ChatMessage message)
  {
    if (message.UserID != _userID)
    {
      _view.DisplayUserMessageFromServer(message.Message, message.UserID);
    }
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public Task OnEventReceivedAsync(UserEventMessage message)
  {
    if (message.ForUserID != _userID)
    {
      string displayMessage;
      if ("Join".EqualsAnyCase(message.EventType))
      {
        displayMessage = $"{message.ForUserID} joined the channel.";
      }
      else if ("Leave".EqualsAnyCase(message.EventType))
      {
        displayMessage = $"{message.ForUserID} left the channel.";
      }
      else
      {
        displayMessage = $"Unknown Event {message.EventType} occurred for {message.ForUserID}.";
      }

      _view.DisplayMessageFromServer(displayMessage);
    }
    return Task.CompletedTask;
  }
}
