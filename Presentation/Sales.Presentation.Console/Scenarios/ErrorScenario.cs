using FluentValidation.Results;
using Spectre.Console;
using Spectre.Console.Json;

namespace Sales.Presentation.Console.Scenarios;

public sealed class ErrorScenario
{
    public static void ShowValidationErrors(List<ValidationFailure> errors)
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

        AnsiConsole.Markup("[bold red]AppSettings configuration is invalid:[/]");
        foreach (var error in errors)
        {
            AnsiConsole.MarkupLine($"[red]• {error.ErrorMessage}[/]");
        }

        AnsiConsole.Write(
            new Panel(json)
                .Header("Config [underline red]have to look[/] like template:")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Green));
    }
}