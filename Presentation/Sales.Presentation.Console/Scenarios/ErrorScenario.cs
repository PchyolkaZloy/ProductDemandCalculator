using FluentValidation.Results;
using Spectre.Console;
using Spectre.Console.Json;

namespace Sales.Presentation.Console.Scenarios;

public sealed class ErrorScenario
{
    public void ShowValidationErrors(List<ValidationFailure> errors)
    {
        var json = new JsonText(
            """
            { 
                "AppSettings": {
                    "ParallelismDegree": 1,
                    "ChannelWriterCapacityInMb": 10,
                    "ChannelReaderCapacityInMb": 10,
                    "InputFilePath": "input.csv",
                    "OutputFilePath": "output.csv"
                }
            }
            """);
        
        AnsiConsole.MarkupLine("[bold red]Validation failed with the following errors:[/]");
        foreach (var error in errors)
        {
            AnsiConsole.MarkupLine($"[red]• {error.ErrorMessage}[/]");
        }

        AnsiConsole.Write(
            new Panel(json)
                .Header("Config JSON [underline red]must look[/] like:")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Green));
    }
}