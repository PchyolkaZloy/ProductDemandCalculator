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
        progressTracker.MaxValue = TotalLines(reader);

        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var productInfo = csv.GetRecord<ProductInfo>();
            progressTracker.IncrementRead();
            
            await channelWriter.WriteAsync(productInfo, cancellationToken);
        }

        channelWriter.Complete();
    }

    private static long TotalLines(StreamReader reader)
    {
        const int headerRows = 1;
        var counter = 0;

        while (reader.ReadLine() != null)
        {
            counter++;
        }

        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();

        return counter - headerRows;
    }
}