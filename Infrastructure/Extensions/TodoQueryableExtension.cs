
namespace Core.Entities.Extensions;

public static class TodoQueryableExtension
{
    public static IQueryable<TodoEntity> FindByGuid(this IQueryable<TodoEntity> source, Guid guid)
    {
        return source.Where(i => i.Guid == guid);
    }
}