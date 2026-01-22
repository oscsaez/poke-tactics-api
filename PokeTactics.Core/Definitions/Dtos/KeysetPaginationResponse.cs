namespace PokeTactics.Core.Definitions.Dtos;

public record KeysetPaginationResponse<T>(
    IEnumerable<T> Items,
    int? NextLastId
);
