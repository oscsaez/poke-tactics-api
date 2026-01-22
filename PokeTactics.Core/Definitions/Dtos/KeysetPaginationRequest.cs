namespace PokeTactics.Core.Definitions.Dtos;

public record KeysetPaginationRequest(
    int PageSize,
    int? LastId
);
