using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.Providers;

// Kind of uncertain on injecting IServiceProvider https://deviq.com/antipatterns/service-locator
public class TodoDataProviderFactory(IServiceProvider serviceProvider) : ITodoDataProviderFactory
{
    public ITodoDataProvider GetTodoProvider(TodoDataProviderType providerType)
    {
        return providerType switch
        {
            TodoDataProviderType.SqlServer => serviceProvider.GetRequiredService<SqlServerTodoProvider>(),
            TodoDataProviderType.MongoDb => serviceProvider.GetRequiredService<MongoDbTodoProvider>(),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, "Unsupported provider type.")
        };
    }
}