using System;
using PokeTactics.Contracts.Stat.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class StatMapper
{
    public static Stat ToStat(this StatPokeApiResponse statPokeApiResponse)
    {
        return new Stat
        {
            Base = statPokeApiResponse.Base,
            Name = statPokeApiResponse.Stat.Name
        };
    }

    public static ICollection<Stat> ToStats(this ICollection<StatPokeApiResponse> statPokeApiResponses) => [..statPokeApiResponses.Select(ToStat)];
}
