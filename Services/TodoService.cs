using Microsoft.EntityFrameworkCore;
using Rest_API_service.Data;
using Rest_API_service.DTOs;
using Rest_API_service.Models;

namespace Rest_API_service.Services;

public class TodoService(AppDbContext dbContext) : ITodoService
{
    public async Task<TodoResponse> CreateAsync(Guid userId, TodoCreateRequest request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            OwnerId = userId,
            Title = request.Title,
            Description = request.Description
        };

        dbContext.TodoItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<PagedResult<TodoResponse>> GetPageAsync(Guid userId, int page, int pageSize, bool? isCompleted, string? search, CancellationToken cancellationToken)
    {
        var query = dbContext.TodoItems.AsNoTracking().Where(x => x.OwnerId == userId);

        if (isCompleted.HasValue)
            query = query.Where(x => x.IsCompleted == isCompleted.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Title.Contains(search) || (x.Description ?? string.Empty).Contains(search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query.OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<TodoResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TodoItems.AsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerId == userId && x.Id == id, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<TodoResponse?> UpdateAsync(Guid userId, Guid id, TodoUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.OwnerId == userId && x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.IsCompleted = request.IsCompleted;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.OwnerId == userId && x.Id == id, cancellationToken);
        if (entity is null) return false;

        dbContext.TodoItems.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TodoResponse Map(TodoItem item) => new(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAtUtc, item.UpdatedAtUtc);
}
