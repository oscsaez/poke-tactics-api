using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils.Attributes;

namespace PokeTactics.Test.Core.Utils.Attributes;

public class MaxListCountAttributeTest
{
    private const int MaxListCount = 1;

    [Fact]
    public void MaxListCountAttribute_CountDoesNotExceedMax()
    {
        // Arrange
        var sample = new Sample
        {
            SampleCollection = [1]
        };

        var context = new ValidationContext(sample);
        var results = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(sample, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void MaxListCountAttribute_CountExceedsMax()
    {
        // Arrange
        string expectedErrorMessage = $"The list cannot contain more than {MaxListCount} elements.";

        var sample = new Sample
        {
            SampleCollection = [1, 2]
        };

        var context = new ValidationContext(sample);
        var results = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(sample, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        ValidationResult result = Assert.Single(results);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
    }

    private class Sample
    {
        [MaxListCount(MaxListCount)]
        public ICollection<int> SampleCollection { get; set; } = [];
    }
}
