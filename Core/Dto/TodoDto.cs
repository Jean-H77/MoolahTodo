using Core.Entities;

namespace Core.Dto;

public class TodoDto
{
    public required string Title { get; set; }
    
    public bool IsComplete { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid Guid { get; set; }
    
    public static TodoEntity ToTodoEntity(TodoDto dto)
    {
        return new TodoEntity
        {
            Title = dto.Title,
            IsComplete = dto.IsComplete,
            DueDate = dto.DueDate,
            Guid = dto.Guid
        };
    }
    
    public static MongoTodoEntity ToMongoTodoEntity(TodoDto dto)
    {
        return new MongoTodoEntity
        {
            Title = dto.Title,
            IsComplete = dto.IsComplete,
            DueDate = dto.DueDate,
            Guid = dto.Guid
        };
    }
}