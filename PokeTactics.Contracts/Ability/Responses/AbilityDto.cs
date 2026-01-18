using System;

namespace PokeTactics.Contracts.Ability.Responses;

public class AbilityDto
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsHidden { get; set; }
}
