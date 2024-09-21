using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Services.Extensions;
using Sales.Infrastructure.FileIO.Extensions;
using Sales.Presentation.Console.Config;
using Sales.Presentation.Console.Extensions;
using Sales.Presentation.Console.Runners;
using Spectre.Console;

namespace Sales.Presentation.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // TODO проверка args
        const string configPath =
            "/home/pchz/JetBrains/projects/RiderProjects/Ozon/Homeworks/homework-3/Presentation/Sales.Presentation.Console/appsettings.json";

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var appOptions = new AppSettingsConfig();
        configuration.GetSection("AppSettings").Bind(appOptions);

        var services = new ServiceCollection();

        services.AddOptions();
        services.Configure<AppSettingsConfig>(configuration.GetSection("AppSettings"));

        // TODO проверка AppSettingsConfig

        services
            .AddApplicationServices(appOptions.ChannelReaderCapacityInMb, appOptions.ChannelWriterCapacityInMb)
            .AddInfrastructureFileIO(appOptions.InputFilePath, appOptions.OutputFilePath)
            .AddPresentationConsole();

        var serviceProvider = services.BuildServiceProvider();
        var cts = serviceProvider.GetRequiredService<CancellationTokenSource>();
        var consoleScenarioRunner = serviceProvider.GetRequiredService<ConsoleScenarioRunner>();

        System.Console.CancelKeyPress += (s, eventArgs) =>
        {
            AnsiConsole.Markup("[red]Cancelling...[/]");
            consoleScenarioRunner.CancelProcessing();
            eventArgs.Cancel = true;
        };

        await consoleScenarioRunner.ExecuteAsync(cts.Token);
    }
}