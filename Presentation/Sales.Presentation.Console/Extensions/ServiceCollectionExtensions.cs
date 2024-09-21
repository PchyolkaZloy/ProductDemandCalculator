using Microsoft.Extensions.DependencyInjection;
using Sales.Presentation.Console.Scenarios;

namespace Sales.Presentation.Console.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationConsole(this IServiceCollection collection)
    {
        collection.AddTransient<ProgressScenario>();

        collection.AddSingleton(new CancellationTokenSource());

        return collection;
    }
}