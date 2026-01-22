namespace PokeTactics.Contracts.Move.Responses;

public class MoveDto
{
    public required string Name { get; set; }

    public int? Power { get; set; }

    public int? Accuracy { get; set; }

    /// <summary>
    /// The times that the move can be used
    /// </summary>
    public int? PowerPoints { get; set; }

    public string? Description { get; set; }

    public required string Type { get; set; }
}
