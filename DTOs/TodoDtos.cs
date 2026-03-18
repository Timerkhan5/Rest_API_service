namespace Rest_API_service.DTOs;

public record TodoCreateRequest(string Title, string? Description);

public record TodoUpdateRequest(string Title, string? Description, bool IsCompleted);

public record TodoResponse(Guid Id, string Title, string? Description, bool IsCompleted, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

public class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
