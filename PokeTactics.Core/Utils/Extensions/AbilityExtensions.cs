using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Utils.Extensions;

public static class AbilityExtensions
{
    public static bool Compare(this Ability ability1, Ability ability2)
    {
        return ability1.Name == ability2.Name
            && ability1.Description == ability2.Description;
    }
}
