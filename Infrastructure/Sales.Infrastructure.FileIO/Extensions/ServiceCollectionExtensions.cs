using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sales.Application.Services.Config;
using Sales.Domain.Interfaces.FileIO;
using Sales.Domain.Interfaces.Service;
using Sales.Infrastructure.FileIO.Readers;
using Sales.Infrastructure.FileIO.Writers;

namespace Sales.Infrastructure.FileIO.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureFileIO(this IServiceCollection collection)
    {
        collection.AddSingleton<IFileReader>(
            provider =>
            {
                var tracker = provider.GetRequiredService<IProgressTracker>();
                var options = provider.GetRequiredService<IOptionsMonitor<AppSettingsConfig>>();

                return new CsvFileReader(tracker, options.CurrentValue.InputFilePath);
            });

        collection.AddSingleton<IFileWriter>(
            provider =>
            {
                var tracker = provider.GetRequiredService<IProgressTracker>();
                var options = provider.GetRequiredService<IOptionsMonitor<AppSettingsConfig>>();

                return new CsvFileWriter(tracker, options.CurrentValue.OutputFilePath);
            });

        return collection;
    }
}