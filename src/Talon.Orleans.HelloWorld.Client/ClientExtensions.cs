namespace Talon.Orleans.HelloWorld.Client;

public static class ClientExtensions
{
  public static bool EqualsAnyCase(this string source, string? compareTo)
  { 
    return source.Equals(compareTo, StringComparison.InvariantCultureIgnoreCase);
  }
}