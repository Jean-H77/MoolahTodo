using Core.Dto;

namespace Core.Models;

public class PaginatedTodoResult
{
    public required List<TodoDto> Todos { get; set; }

    public int TotalCount { get; set; }
}