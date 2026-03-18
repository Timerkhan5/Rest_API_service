using Rest_API_service.DTOs;

namespace Rest_API_service.Services;

public interface ITodoService
{
    Task<TodoResponse> CreateAsync(Guid userId, TodoCreateRequest request, CancellationToken cancellationToken);
    Task<PagedResult<TodoResponse>> GetPageAsync(Guid userId, int page, int pageSize, bool? isCompleted, string? search, CancellationToken cancellationToken);
    Task<TodoResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<TodoResponse?> UpdateAsync(Guid userId, Guid id, TodoUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}
