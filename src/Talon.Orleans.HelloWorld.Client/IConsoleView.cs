
namespace Talon.Orleans.HelloWorld.Client;

/// <summary>
/// UI abstraction over the Console.
/// </summary>
public interface IConsoleView
{
  /// <summary>Displays a status message to the Console</summary>
  void Status(string message);

  /// <summary>Displays an information message to the Console.</summary>
  void Info(string message);

  /// <summary>
  /// Prompts the user for their UserID. If no valid user is specified then null is returned.
  /// </summary>
  string? PromptForUserID();

  /// <summary>Prompts the user for input.</summary>
  string? GetUserInput();

  /// <summary>
  /// Receives a message from the server and then displays the result to the screen.
  /// </summary>
  void DisplayMessageFromServer(string message);

  /// <summary>
  /// Receives a message from the server and displays the username and message.
  /// </summary>
  void DisplayUserMessageFromServer(string message, string userName);
}