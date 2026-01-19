using System;
using PokeTactics.Contracts.Stat.PokeApi;
using PokeTactics.Contracts.Stat.Responses;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class StatMapper
{
    // DTO -> Entity
    public static Stat ToStat(this StatPokeApiResponse statPokeApiResponse)
    {
        return new Stat
        {
            Base = statPokeApiResponse.Base,
            Name = statPokeApiResponse.Stat.Name
        };
    }

    public static ICollection<Stat> ToStats(this ICollection<StatPokeApiResponse> statPokeApiResponses) => [..statPokeApiResponses.Select(ToStat)];

    // Entity -> DTO
    public static StatDto ToStatDto(this Stat stat)
    {
        return new StatDto
        {
            Base = stat.Base,
            Name = stat.Name
        };
    }

    public static ICollection<StatDto> ToStatDtos(this ICollection<Stat> stats) => [..stats.Select(ToStatDto)];
}
