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
        long counter = 0;

        var buffer = new char[8192]; // 16Kb
        int charsRead;

        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < charsRead; i++)
            {
                if (buffer[i] == '\n')
                {
                    counter++;
                }
            }
        }

        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();

        return counter - headerRows;
    }
}