using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace PokeTactics.Core.Utils.Attributes
{
    // TODO: If I am not using this attributes in requests in controllers/minimal APIs I shoul validate manually (Validator.ValidateObject())
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxListCountAttribute : ValidationAttribute
    {
        private readonly int _maxCount;

        public MaxListCountAttribute(int maxCount)
        {
            _maxCount = maxCount;
            ErrorMessage = $"The list cannot contain more than {_maxCount} elements.";
        }

        public override bool IsValid(object? value)
        {
            if (value is ICollection collection && collection.Count > _maxCount)
            {
                return false;
            }

            return true;
        }
    }
}