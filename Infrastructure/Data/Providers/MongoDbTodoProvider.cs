using Core.Dto;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Data.Providers;

public class MongoDbTodoProvider(MongoDbContext db) : ITodoDataProvider
{

    public async Task<PaginatedTodoResult> GetAllTodos(int page)
    {
        var totalCount = await db.TodoCollection.CountDocumentsAsync(FilterDefinition<MongoTodoEntity>.Empty);
        var mongoTodoEntities = await GetPaginatedListAsync(db.TodoCollection, FilterDefinition<MongoTodoEntity>.Empty, page);
        
        return new PaginatedTodoResult
        {
            TotalCount = (int)totalCount, // I don't think an integer overflow will happen
            Todos = mongoTodoEntities.Select(MongoTodoEntity.ToDto).ToList()
        };
    }

    public async Task<PaginatedTodoResult> GetTodosBySearchAsync(string search, int page)
    {
        var filter = string.IsNullOrWhiteSpace(search)
            ? FilterDefinition<MongoTodoEntity>.Empty
            : Builders<MongoTodoEntity>.Filter.Regex(
                entity => entity.Title,
                new BsonRegularExpression(search, "i")); 

        var totalCount = await db.TodoCollection.CountDocumentsAsync(filter);
        var mongoTodoEntities = await GetPaginatedListAsync(db.TodoCollection, filter, page);

        return new PaginatedTodoResult
        {
            TotalCount = (int)totalCount,
            Todos = mongoTodoEntities.Select(MongoTodoEntity.ToDto).ToList()
        };
    }

    private static async Task<List<MongoTodoEntity>> GetPaginatedListAsync(
        IMongoCollection<MongoTodoEntity> collection,
        FilterDefinition<MongoTodoEntity> filter,
        int page,
        int pageSize = 10)
    {
        return await collection.Find(filter)
            .SortBy(x => x.Id) 
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
    
    public async Task<TodoDto?> GetTodoByGuidAsync(Guid guid)
    {
        var filter = Builders<MongoTodoEntity>.Filter.Eq(x => x.Guid, guid);
        var mongoTodoEntity = await db.TodoCollection.Find(filter).FirstOrDefaultAsync();

        return mongoTodoEntity == null ? null : MongoTodoEntity.ToDto(mongoTodoEntity);
    }

    public async Task<TodoDto> AddTodoAsync(TodoDto todo)
    {
        var mongoTodoEntity = TodoDto.ToMongoTodoEntity(todo);
        mongoTodoEntity.Guid = Guid.NewGuid();
        
        await db.TodoCollection.InsertOneAsync(mongoTodoEntity);
        
        return MongoTodoEntity.ToDto(mongoTodoEntity);
    }

    public async Task<int> DeleteTodoByGuidAsync(Guid guid)
    {
        var filter = Builders<MongoTodoEntity>.Filter.Eq(entity => entity.Guid, guid);
        return (int)(await db.TodoCollection.DeleteOneAsync(filter)).DeletedCount;
    }
    
    public async Task<int> UpdateTodoAsync(TodoDto todo)
    {
        var filter = Builders<MongoTodoEntity>.Filter.Eq(entity => entity.Guid, todo.Guid);

        var update = Builders<MongoTodoEntity>.Update
            .Set(entity => entity.Title, todo.Title)
            .Set(entity => entity.IsComplete, todo.IsComplete)
            .Set(entity => entity.DueDate, todo.DueDate);

        return (int)(await db.TodoCollection.UpdateOneAsync(filter, update)).ModifiedCount;
    }
}