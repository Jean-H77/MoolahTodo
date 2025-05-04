using Core.Entities;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Data;

// https://www.youtube.com/watch?v=jJK9alBkzU0
public class MongoDbContext // Not sure if this is the correct name for this
{
    public IMongoCollection<MongoTodoEntity> TodoCollection { get; }
    
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionUri);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        TodoCollection = database.GetCollection<MongoTodoEntity>(settings.Value.CollectionName);
        
        var keys = Builders<MongoTodoEntity>.IndexKeys.Text(x => x.Title);
        TodoCollection.Indexes.CreateOneAsync(new CreateIndexModel<MongoTodoEntity>(keys));
    }
}