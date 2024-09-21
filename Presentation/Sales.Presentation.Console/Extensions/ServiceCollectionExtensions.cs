using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sales.Presentation.Console.Config;
using Sales.Presentation.Console.Runners;
using Sales.Presentation.Console.Scenarios;
using Sales.Presentation.Console.Validators;

namespace Sales.Presentation.Console.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationConsole(this IServiceCollection collection)
    {
        collection.AddTransient<IValidator<AppSettingsConfig>, AppSettingsConfigValidator>();
        
        collection.AddTransient<ProgressScenario>();
        collection.AddTransient<ErrorScenario>();
        collection.AddTransient<ConsoleScenarioRunner>();
        
        collection.AddSingleton(new CancellationTokenSource());

        return collection;
    }
}