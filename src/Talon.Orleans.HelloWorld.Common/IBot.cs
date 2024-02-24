namespace Talon.Orleans.HelloWorld.Common;

public interface IBot : IGrainWithStringKey
{
  /// <summary>The quintessential Hello World.</summary>
  ValueTask<string> SayHello(string clientName);
}