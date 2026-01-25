namespace PokeTactics.Contracts.Common.Requests;

public record KeysetPaginationRequest(
    int PageSize,
    int? LastPokedexOrder,
    int? LastId
)
{
    public bool HasInvalidCursor => LastPokedexOrder.HasValue ^ LastId.HasValue;
};
