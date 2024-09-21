using Microsoft.Extensions.Options;
using Sales.Domain.Interfaces.Service;
using Sales.Presentation.Console.Config;
using Sales.Presentation.Console.CustomColumns;
using Spectre.Console;

namespace Sales.Presentation.Console.Scenarios;

public sealed class ProgressScenario
{
    private readonly IProductProcessService _productProcessService;
    private readonly IProgressTracker _tracker;
    private readonly IOptionsMonitor<AppSettingsConfig> _optionsMonitor;

    public ProgressScenario(
        IProductProcessService productProcessService,
        IProgressTracker tracker,
        IOptionsMonitor<AppSettingsConfig> optionsMonitor
    )
    {
        _productProcessService = productProcessService;
        _optionsMonitor = optionsMonitor;
        _tracker = tracker;

        _optionsMonitor.OnChange(UpdateParallelismDegree);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var serviceTask = _productProcessService.StartProcessingAsync(
                _optionsMonitor.CurrentValue.ParallelismDegree,
                cancellationToken
            );

            var consoleProgress = AnsiConsole
                .Progress()
                .AutoClear(false)
                .HideCompleted(false)
                .Columns(
                    new SpinnerColumn(),
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new ElapsedTimeColumn(),
                    new AmountColumn()
                );

            var consoleProgressTask = consoleProgress
                .StartAsync(async ctx =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var readProgress = ctx.AddTask("[green]Read[/]")
                        .MaxValue(_tracker.MaxValue);
                    var completeTask = ctx.AddTask("[green]Completed[/]").MaxValue(_tracker.MaxValue);
                    var writeProgress =
                        ctx.AddTask("[green]Wrote[/]")
                            .MaxValue(_tracker.MaxValue);


                    while (!ctx.IsFinished)
                    {
                        await Task.Delay(1, cancellationToken);
                        readProgress.Value(_tracker.Read);
                        completeTask.Value(_tracker.Completed);
                        writeProgress.Value(_tracker.Wrote);
                    }

                    return Task.CompletedTask;
                });

            await Task.WhenAll(serviceTask, consoleProgressTask);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.Markup("[red]Cancelled[/]");
        }
    }
    

    private void UpdateParallelismDegree(AppSettingsConfig config)
    {
        _productProcessService.UpdateParallelismDegree(config.ParallelismDegree);
    }
}