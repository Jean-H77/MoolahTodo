using Core.Dto;
using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MoolahTodo.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/todos");
        
        group.MapGet("/", async (string provider, int page, ITodoDataProviderFactory dataProvider) =>
        {
            return await GetMappedTodosAsync(provider, dataProvider, p => p.GetAllTodos(page));
        });
        
        group.MapGet("/search", async (string search, int page, string provider, ITodoDataProviderFactory dataProvider) =>
        {
            return await GetMappedTodosAsync(provider, dataProvider, p => p.GetTodosBySearchAsync(search, page));
        });
        
        group.MapGet("{id:guid}", async Task<Results<NotFound, Ok<TodoDto>>> (Guid guid, string provider, ITodoDataProviderFactory dataProvider) =>
        {
            var todoProvider = GetProviderFromString(dataProvider, provider);
            var todo = await todoProvider.GetTodoByGuidAsync(guid);

            return todo == null ? TypedResults.NotFound() : TypedResults.Ok(todo);
        });
        
        group.MapDelete("{id:guid}", async Task<Results<BadRequest, Ok>> (Guid id, string provider, ITodoDataProviderFactory dataProvider) =>
        {
            var todoProvider = GetProviderFromString(dataProvider, provider);
            var rowsAffected = await todoProvider.DeleteTodoByGuidAsync(id);     
            
            return rowsAffected == 0 ? TypedResults.BadRequest() : TypedResults.Ok();
        });
        
        group.MapPost("/", async Task<Results<BadRequest, Ok<TodoDto>, ValidationProblem>> 
            (IValidator<TodoDto> validator, TodoDto todo, string provider, ITodoDataProviderFactory dataProvider) =>
        {
            var validationResult = await validator.ValidateAsync(todo);

            if (!validationResult.IsValid) 
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var todoProvider = GetProviderFromString(dataProvider, provider);
    
            try
            {
                var addedTodo = await todoProvider.AddTodoAsync(todo);
                return TypedResults.Ok(addedTodo);
            }
            catch (Exception ignore)
            {
                return TypedResults.BadRequest();
            }
        });
        
        group.MapPut("/", async Task<Results<NotFound, Ok, ValidationProblem>> (IValidator<TodoDto> validator, TodoDto todo, string provider, ITodoDataProviderFactory dataProvider) =>
        {
            var validationResult = await validator.ValidateAsync(todo);

            if (!validationResult.IsValid) 
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var todoProvider = GetProviderFromString(dataProvider, provider);
            var rowsAffected = await todoProvider.UpdateTodoAsync(todo);
         
            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });
    }
    
    private static ITodoDataProvider GetProviderFromString(ITodoDataProviderFactory factory, string provider)
    {
        var providerType = Enum.TryParse<TodoDataProviderType>(provider, true, out var parsed)
            ? parsed
            : TodoDataProviderType.SqlServer; 

        return factory.GetTodoProvider(providerType);
    }
    
    // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/using-delegates
    private static async Task<IResult> GetMappedTodosAsync(
        string provider, 
        ITodoDataProviderFactory todoProviderFactory, 
        Func<ITodoDataProvider, Task<PaginatedTodoResult>> fetchTodos)
    {
        var todoProvider = GetProviderFromString(todoProviderFactory, provider);
        var result = await fetchTodos(todoProvider);
        return Results.Ok(result); 
    }
}