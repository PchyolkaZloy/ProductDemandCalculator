using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Services.Extensions;
using Sales.Infrastructure.FileIO.Extensions;
using Sales.Presentation.Console.Config;
using Sales.Presentation.Console.Extensions;
using Sales.Presentation.Console.Scenarios;
using Sales.Presentation.Console.Validators;
using Spectre.Console;

namespace Sales.Presentation.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            AnsiConsole.Markup(
                "[bold red]Error:[/] You must provide the path to the configuration file as an argument.");
            return;
        }

        var configPath = args[0];

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var appOptions = new AppSettingsConfig();
        configuration.GetSection("AppSettings").Bind(appOptions);

        var validator = new AppSettingsConfigValidator();
        var validationResult = await validator.ValidateAsync(appOptions);
        if (!validationResult.IsValid)
        {
            ErrorScenario.ShowValidationErrors(validationResult.Errors);

            return;
        }

        var services = new ServiceCollection();

        services.AddOptions();
        services.Configure<AppSettingsConfig>(configuration.GetSection("AppSettings"));


        services
            .AddApplicationServices(appOptions.ChannelReaderCapacityInMb, appOptions.ChannelWriterCapacityInMb)
            .AddInfrastructureFileIO(appOptions.InputFilePath, appOptions.OutputFilePath)
            .AddPresentationConsole();

        var serviceProvider = services.BuildServiceProvider();
        var cts = serviceProvider.GetRequiredService<CancellationTokenSource>();
        var progressScenario = serviceProvider.GetRequiredService<ProgressScenario>();

        System.Console.CancelKeyPress += (s, eventArgs) =>
        {
            AnsiConsole.Markup("[red]Cancelling...[/]");
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        await progressScenario.RunAsync(cts);
    }
}