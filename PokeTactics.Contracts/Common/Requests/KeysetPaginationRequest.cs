namespace PokeTactics.Contracts.Common.Requests;

public record KeysetPaginationRequest(
    int PageSize,
    int? LastId
);
