using Core.Dto;

namespace Core.Entities;

public partial class TodoEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsComplete { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid Guid { get; set; }
    
    public static TodoDto ToDto(TodoEntity entity)
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
