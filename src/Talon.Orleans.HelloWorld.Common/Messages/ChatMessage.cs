namespace Talon.Orleans.HelloWorld.Common.Messages;

/// <summary>The Chat message sent to the channel for the given User.</summary>
[GenerateSerializer, Immutable]
public record ChatMessage(
  string UserID,
  string Message,
  DateTime MessageSentUtc);