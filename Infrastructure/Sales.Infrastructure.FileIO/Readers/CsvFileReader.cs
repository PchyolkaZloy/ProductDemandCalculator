using System.Globalization;
using System.Threading.Channels;
using CsvHelper;
using Sales.Domain.Interfaces.FileIO;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Infrastructure.FileIO.Readers;

public sealed class CsvFileReader(IProgressTracker progressTracker, string filePath) : IFileReader
{
    public async Task ReadProductInfosAsync(
        ChannelWriter<ProductInfo> channelWriter,
        CancellationToken cancellationToken
    )
    {
        using var reader = new StreamReader(filePath);
        progressTracker.MaxValue = reader.BaseStream.Length;
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await foreach (var productInfo in csv.GetRecordsAsync<ProductInfo>(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            progressTracker.IncrementRead();

            await channelWriter.WriteAsync(productInfo, cancellationToken);
        }

        channelWriter.Complete();
    }
}