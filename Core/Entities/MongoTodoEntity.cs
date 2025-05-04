using Core.Dto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Entities;

public class MongoTodoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsComplete { get; set; }

    public DateTime? DueDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid Guid { get; set; }
    
    public static TodoDto ToDto(MongoTodoEntity entity)
    {
        return new TodoDto
        {
            Title = entity.Title,
            IsComplete = entity.IsComplete,
            DueDate = entity.DueDate,
            Guid = entity.Guid
        };
    }
}