using Microsoft.Extensions.Logging;
using Talon.Orleans.HelloWorld.Common;

namespace Talon.Orleans.HelloWorld.Grains;

public class Bot : IBot
{
  private readonly ILogger _logger;

  public Bot(ILogger<Bot> logger)
  {
    _logger = logger;
  }

  /// <inheritdoc />
  public ValueTask<string> SayHello(string clientName)
  {
    // log information on the server that the payload was received
    _logger.LogInformation($"Received greeting from: {clientName}");

    // respond back to the client
    string resultMessage = $"Hello {clientName}, I am an Orleans Bot.";
    return ValueTask.FromResult(resultMessage);
  }
}