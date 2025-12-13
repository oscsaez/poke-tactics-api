namespace PokeTactics.Api.Utils;

public class MessagesHelper
{
    public static string GetApiCallFailMessage(string path)
    {
        return $"Something failed calling PokeApi. Path: [{path}]";
    }
}
