namespace PokeTactics.Api.Utils;

public static class MessagesHelper
{
    public static string GetApiCallFailMessage(string path)
    {
        return $"Something failed calling PokeApi. Path: [{path}]";
    }
}
