using Core.Dto;
using Core.Entities;
using Core.Entities.Extensions;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Providers;

public class SqlServerTodoProvider(SqlServerDbContext db) : ITodoDataProvider
{
    //  https://learn.microsoft.com/en-us/ef/core/querying/tracking#no-tracking-queries
    public async Task<PaginatedTodoResult> GetAllTodos(int page)
    {
        var totalCount = await db.TodoEntities.CountAsync();

        var todos = db.TodoEntities.AsNoTracking();
        var paginatedTodos = await GetPaginatedListAsync(todos, page);
        
        return new PaginatedTodoResult
        {
            TotalCount = totalCount,
            Todos = paginatedTodos.Select(TodoEntity.ToDto).ToList()
        };
    }

    public async Task<PaginatedTodoResult> GetTodosBySearchAsync(string search, int page)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(search);
        
        var filteredTodos = db.TodoEntities.AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.Title, $"%{search}%"));
        
        var totalCount = await filteredTodos.CountAsync();
        var paginatedTodos= await GetPaginatedListAsync(filteredTodos, page);
        
        return new PaginatedTodoResult
        {
            TotalCount = totalCount,
            Todos = paginatedTodos.Select(TodoEntity.ToDto).ToList()
        };
    }

    private static async Task<List<TodoEntity>> GetPaginatedListAsync(
        IQueryable<TodoEntity> todos, 
        int page,
        int pageSize = 10)
    {
        return await todos
            .OrderBy(entity => entity.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<TodoDto?> GetTodoByGuidAsync(Guid guid)
    {
        var todoEntity = await db.TodoEntities
            .FindByGuid(guid)
            .FirstOrDefaultAsync();
        
        return todoEntity != null ? TodoEntity.ToDto(todoEntity) : null;
    }
    
    public async Task<TodoDto> AddTodoAsync(TodoDto todo)
    {
        var todoEntity = TodoDto.ToTodoEntity(todo); 
        db.Add(todoEntity);
        var rowsAffected = await db.SaveChangesAsync();

        if (rowsAffected == 1)
            return TodoEntity.ToDto(todoEntity);
        
        throw new Exception("Failed to add todo to the database.");
    }
    
    public async Task<int> DeleteTodoByGuidAsync(Guid guid)
    {
       return await db.TodoEntities
           .FindByGuid(guid)
          .ExecuteDeleteAsync();
    }

    public async Task<int> UpdateTodoAsync(TodoDto todo)
    {
        var todoEntity = TodoDto.ToTodoEntity(todo);
        
        return await db.TodoEntities
            .FindByGuid(todoEntity.Guid) 
            .ExecuteUpdateAsync(updates =>
                updates.SetProperty(entity => entity.IsComplete, todo.IsComplete)
                    .SetProperty(entity => entity.DueDate, todo.DueDate)
                    .SetProperty(entity => entity.Title, todo.Title));
    }
}