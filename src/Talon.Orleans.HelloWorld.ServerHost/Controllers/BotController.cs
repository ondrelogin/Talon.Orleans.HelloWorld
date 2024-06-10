using Microsoft.AspNetCore.Mvc;
using Talon.Orleans.HelloWorld.Common;

namespace Talon.Orleans.HelloWorld.ServerHost.Controllers;

/// <summary>
/// Web API Controller for basic Orleans access.
/// </summary>
[Route("api/bot")]
[ApiController]
public class BotController : ControllerBase
{
  private readonly IClusterClient _grainFactory;

  public BotController(IClusterClient grainFactory)
  {
    _grainFactory = grainFactory;
  }

  /// <summary>calls say hello</summary>
  [Route("sayhello")]
  [HttpGet]
  public async Task<string> SayHelloAsync(string clientName)
  {
    var bot = _grainFactory.GetGrain<IBot>(clientName);
    return await bot.SayHello(clientName);
  }
}