using Core.Enums;

namespace Core.Interfaces;

public interface ITodoDataProviderFactory
{
    ITodoDataProvider GetTodoProvider(TodoDataProviderType providerType);
}