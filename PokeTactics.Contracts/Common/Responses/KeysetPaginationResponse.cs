namespace PokeTactics.Contracts.Common.Responses;

public record KeysetPaginationResponse<T>(
    IEnumerable<T> Items,
    int? NextLastId
);
