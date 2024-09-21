using FluentValidation;
using Microsoft.Extensions.Options;
using Sales.Presentation.Console.Config;
using Sales.Presentation.Console.Scenarios;

namespace Sales.Presentation.Console.Runners;

public sealed class ConsoleScenarioRunner(
    IOptions<AppSettingsConfig> appSettingsConfigOptions,
    IValidator<AppSettingsConfig> validator,
    ProgressScenario progressScenario,
    ErrorScenario errorScenario,
    CancellationTokenSource cts
)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var validationErrors = await validator.ValidateAsync(appSettingsConfigOptions.Value, cancellationToken);

        if (validationErrors.Errors.Count == 0)
        {
            await progressScenario.RunAsync(cancellationToken);
        }
        else
        {
            errorScenario.ShowValidationErrors(validationErrors.Errors);
        }
    }

    public void CancelProcessing()
    {
        cts.Cancel();
    }
}