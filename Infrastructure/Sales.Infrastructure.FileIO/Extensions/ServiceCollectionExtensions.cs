using Microsoft.Extensions.DependencyInjection;
using Sales.Domain.Interfaces.FileIO;
using Sales.Domain.Interfaces.Service;
using Sales.Infrastructure.FileIO.Readers;
using Sales.Infrastructure.FileIO.Writers;

namespace Sales.Infrastructure.FileIO.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureFileIO(
        this IServiceCollection collection,
        string inputFilePAth,
        string outputFilePath
    )
    {
        collection.AddSingleton<IFileReader>(
            provider =>
            {
                var tracker = provider.GetRequiredService<IProgressTracker>();
                return new CsvFileReader(tracker, inputFilePAth);
            });

        collection.AddSingleton<IFileWriter>(
            provider =>
            {
                var tracker = provider.GetRequiredService<IProgressTracker>();
                return new CsvFileWriter(tracker, outputFilePath);
            });

        return collection;
    }
}