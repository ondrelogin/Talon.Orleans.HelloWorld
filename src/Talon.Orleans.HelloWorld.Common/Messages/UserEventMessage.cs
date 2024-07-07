
namespace Talon.Orleans.HelloWorld.Common.Messages;

/// <summary>Information about when a User leaves or joins a Channel.</summary>
[GenerateSerializer, Immutable]
public record UserEventMessage(
  string ForUserID,
  string EventType,
  DateTime EventUtc);