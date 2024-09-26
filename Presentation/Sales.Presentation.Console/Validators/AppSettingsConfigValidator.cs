using FluentValidation;
using Sales.Presentation.Console.Config;

namespace Sales.Presentation.Console.Validators;

public sealed class AppSettingsConfigValidator : AbstractValidator<AppSettingsConfig>
{
    public AppSettingsConfigValidator()
    {
        const int maxParallelismDegree = 100;

        RuleFor(c => c.ParallelismDegree)
            .NotNull()
            .GreaterThan(0)
            .LessThanOrEqualTo(maxParallelismDegree);

        RuleFor(c => c.InputFilePath)
            .NotNull()
            .NotEmpty()
            .Must(File.Exists)
            .WithMessage(c => $"Input file doesn't exist {c.InputFilePath}")
            .Must((c, inputFilePath) =>
                !Path.GetFullPath(inputFilePath)
                    .Equals(Path.GetFullPath(c.OutputFilePath), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Input and output files must be different and should not have the same path.");

        RuleFor(c => c.OutputFilePath)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.ChannelReaderCapacityInMb)
            .NotNull()
            .GreaterThan(0);

        RuleFor(c => c.ChannelWriterCapacityInMb)
            .NotNull()
            .GreaterThan(0);
    }
}