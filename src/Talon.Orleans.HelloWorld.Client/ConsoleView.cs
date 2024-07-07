namespace Talon.Orleans.HelloWorld.Client;

/// <inheritdoc />
/// <remarks>
/// Note, this ConsoleView is not really robust enough to handle a true
/// multi-user experience. The main issue is how we grab the userInput.
/// I don't know enough about Console manipulation for it to truly work.
/// This is just good enough so that you can run multiple console apps
/// on the same machine and get a semi-multi-user experience.
/// 
/// Basically, if a user is typing during the ReadLine input phase, and
/// an "event" comes in, it's not going to be good.
/// </remarks>
public class ConsoleView : IConsoleView
{
  private const string _darkGrayTextColor = "\x1B[1;30m";

  private const string _userTextColor = "\x1B[0;93m";
  private const string _systemTextColor = "\x1B[0;94m";
  private const string _resetTextColor = "\x1B[0;0m"; 

  /// <inheritdoc />
  public void Status(string message)
  { 
    Console.WriteLine("{0}{1:HH:mm:ss} {2}{3}", 
      _darkGrayTextColor,
      DateTime.Now,
      message,
      _resetTextColor);
  }

  /// <inheritdoc />
  public void Info(string message)
  { 
    Console.WriteLine("{0:HH:mm:ss} {1}", 
      DateTime.Now,
      message);
  }

  /// <inheritdoc />
  public string? PromptForUserID()
  {
    Console.Write("Enter UserID: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) return null;

    return userInput;
  }

  /// <inheritdoc />
  public string? GetUserInput()
  {
    Console.Write("       > ");
    return Console.ReadLine();
  }

  /// <inheritdoc />
  public void DisplayMessageFromServer(string message)
  {
    // the code here isn't perfect, because if a user is typing or has typed something
    //  and someone sends a message it will get mixed up, but is good enough for a demo
    //  as the point of this is not to have a great console UX, but to showcase orleans
    var pos = Console.GetCursorPosition();
    Console.SetCursorPosition(0, pos.Top);

    Console.WriteLine("{2:HH:mm:ss} {0}{3}{1}", 
      _systemTextColor,
      _resetTextColor,
      DateTime.Now, 
      message);
    Console.Write("       > ");
  }

  /// <inheritdoc />
  /// <remarks>Same issues as <see cref="DisplayMessageFromServer"/></remarks>
  public void DisplayUserMessageFromServer(string message, string userName)
  {
    var pos = Console.GetCursorPosition();
    Console.SetCursorPosition(0, pos.Top);

    Console.WriteLine("{2:HH:mm:ss} {0}{3}:{1} {4}", 
      _userTextColor,
      _resetTextColor,
      DateTime.Now, 
      userName, 
      message);
    Console.Write("       > ");
  }
}