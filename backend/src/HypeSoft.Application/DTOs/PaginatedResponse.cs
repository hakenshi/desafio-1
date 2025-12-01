namespace HypeSoft.Application.DTOs;

public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
