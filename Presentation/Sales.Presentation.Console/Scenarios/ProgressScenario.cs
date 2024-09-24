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

    public async Task RunAsync(CancellationTokenSource cts)
    {
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

        var serviceTask = _productProcessService.StartProcessingAsync(
            _optionsMonitor.CurrentValue.ParallelismDegree,
            cts.Token
        );


        var consoleProgressTask = consoleProgress
            .StartAsync(async ctx =>
            {
                cts.Token.ThrowIfCancellationRequested();

                var readProgress = ctx.AddTask("[green]Read[/]")
                    .MaxValue(_tracker.MaxValue);
                var completeTask = ctx.AddTask("[green]Completed[/]")
                    .MaxValue(_tracker.MaxValue);
                var writeProgress = ctx.AddTask("[green]Wrote[/]")
                    .MaxValue(_tracker.MaxValue);


                while (!ctx.IsFinished)
                {
                    await Task.Delay(50, cts.Token);
                    readProgress.Value(_tracker.Read);
                    completeTask.Value(_tracker.Completed);
                    writeProgress.Value(_tracker.Wrote);
                }

                return Task.CompletedTask;
            });

        try
        {
            await serviceTask;
            await consoleProgressTask;
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.Markup("[red]Cancelled[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.Markup($"[red]Error occurred: {ex.Message.EscapeMarkup()}[/]");
            await cts.CancelAsync();
        }

        finally
        {
            await cts.CancelAsync();
        }
    }


    private void UpdateParallelismDegree(AppSettingsConfig config)
    {
        _productProcessService.UpdateParallelismDegree(config.ParallelismDegree);
    }
}