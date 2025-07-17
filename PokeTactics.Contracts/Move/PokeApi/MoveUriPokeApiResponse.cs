using System;

namespace PokeTactics.Contracts.Move.PokeApi;

public class MoveUriPokeApiResponse
{
    public required string Name { get; set; }

    public required string Url { get; set; }
}
