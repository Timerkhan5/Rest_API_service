using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest_API_service.DTOs;
using Rest_API_service.Services;

namespace Rest_API_service.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public class TodoController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TodoResponse>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isCompleted = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var userId = GetUserId();
        var result = await todoService.GetPageAsync(userId, page, pageSize, isCompleted, search, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await todoService.GetByIdAsync(userId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create(TodoCreateRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await todoService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TodoResponse>> Update(Guid id, TodoUpdateRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await todoService.UpdateAsync(userId, id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var deleted = await todoService.DeleteAsync(userId, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException("No user id claim.");
        return Guid.Parse(userId);
    }
}
