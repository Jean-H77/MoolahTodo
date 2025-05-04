using Core.Dto;
using Core.Models;

namespace Core.Interfaces;

public interface ITodoDataProvider
{
    Task<PaginatedTodoResult> GetAllTodos(int page);
    Task<PaginatedTodoResult> GetTodosBySearchAsync(string search, int page);
    Task<TodoDto?> GetTodoByGuidAsync(Guid guid);
    Task<TodoDto> AddTodoAsync(TodoDto todo);
    Task<int> DeleteTodoByGuidAsync(Guid guid);
    Task<int> UpdateTodoAsync(TodoDto todo);
}