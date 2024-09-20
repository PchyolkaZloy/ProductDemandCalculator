using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sales.Application.Services.Config;
using Sales.Application.Services.Extensions;
using Sales.Domain.Interfaces.Service;
using Sales.Infrastructure.FileIO.Extensions;
using Sales.Presentation.Console.Column;
using Spectre.Console;

namespace Sales.Presentation.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        const string configPath =
            "/home/pchz/JetBrains/projects/RiderProjects/Ozon/Homeworks/homework-3/Presentation/Sales.Presentation.Console/appsettings.json";

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddOptions();
        services.Configure<AppSettingsConfig>(configuration.GetSection("AppSettings"));

        services
            .AddApplicationServices()
            .AddInfrastructureFileIO();

        var serviceProvider = services.BuildServiceProvider();
        var productService = serviceProvider.GetRequiredService<IProductProcessService>();
        var tracker = serviceProvider.GetRequiredService<IProgressTracker>();

        System.Console.CancelKeyPress += async (s, eventArgs) =>
        {
            System.Console.WriteLine("Cancelling.");
            productService.CancelProcessing();
            eventArgs.Cancel = true;
        };

        var consoleProgress = AnsiConsole.Progress()
            .AutoClear(false) // Do not remove the task list when done
            .HideCompleted(false) // Hide tasks as they are completed
            .Columns(new ProgressColumn[]
            {
                new SpinnerColumn(), // Spinner
                new TaskDescriptionColumn(), // Task description
                new ProgressBarColumn(), // Progress bar
                new PercentageColumn(), // Percentage
                new ElapsedTimeColumn(),
                new AmountColumn()
            });

        var mainTask = productService.StartProcessingAsync();

        var consoleTask = consoleProgress
            .StartAsync(async ctx =>
            {
                var readProgress = ctx.AddTask("[green]Read[/] :open_book: :open_book: :open_book:").MaxValue(100000);
                var writeProgress = ctx.AddTask("[green]Wrote[/] :writing_hand: :writing_hand: :writing_hand: :writing_hand:").MaxValue(100000);
                var completeTask = ctx.AddTask("[green]Completed[/] :abacus:").MaxValue(100000);

                while (!ctx.IsFinished)
                {
                    await Task.Delay(10);
                    readProgress.Value(tracker.Read);
                    writeProgress.Value(tracker.Wrote);
                    completeTask.Value(tracker.Completed);
                }

                return Task.CompletedTask;
            });

        await Task.WhenAll(mainTask, consoleTask);

        //System.Console.WriteLine($"{tracker.Read} {tracker.Completed} {tracker.Wrote} {tracker.MaxValue}");
    }
}