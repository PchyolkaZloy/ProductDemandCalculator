using System.Globalization;
using System.Threading.Channels;
using CsvHelper;
using Sales.Domain.Interfaces.FileIO;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Infrastructure.FileIO.Writers;

public sealed class CsvFileWriter(IProgressTracker progressTracker, string filePath) : IFileWriter
{
    public async Task WriteProductResultsAsync(
        ChannelReader<ProductResult> channelReader,
        CancellationToken cancellationToken
    )
    {
        await using var writer = new StreamWriter(filePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteHeader(typeof(ProductResult));
        await csv.NextRecordAsync();

        await foreach (var result in channelReader.ReadAllAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            csv.WriteRecord(result);
            progressTracker.IncrementWritten();

            await csv.NextRecordAsync();
        }
    }
}