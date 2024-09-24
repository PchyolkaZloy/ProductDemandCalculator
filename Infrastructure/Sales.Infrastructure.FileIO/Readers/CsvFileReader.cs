using System.Globalization;
using System.Threading.Channels;
using CsvHelper;
using CsvHelper.Configuration;
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

        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
        });

        try
        {
            await csv.ReadAsync();
            if (!csv.ReadHeader())
            {
                throw new InvalidOperationException("Invalid or missing CSV header.");
            }

            var expectedHeaders = new[] { "id", "prediction", "stock" };
            var actualHeaders = csv.HeaderRecord;

            if (actualHeaders.Length != expectedHeaders.Length ||
                !actualHeaders.SequenceEqual(expectedHeaders, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("CSV headers are incorrect.");
            }

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (csv.Context.Parser.RawRecord.Trim().Length == 0)
                {
                    continue;
                }

                var fieldCount = csv.Context.Parser.Record.Length;
                if (fieldCount != expectedHeaders.Length)
                {
                    throw new InvalidOperationException($"Invalid number of fields in row {csv.Context.Parser.Row}.");
                }

                var productInfo = csv.GetRecord<ProductInfo>();
                progressTracker.IncrementRead();

                await channelWriter.WriteAsync(productInfo, cancellationToken);
            }
        }
        finally
        {
            channelWriter.Complete();
        }
    }

    private static long TotalLines(StreamReader reader)
    {
        const int headerRows = 1;
        long counter = 0;

        var buffer = new char[8192]; // 16Kb
        int charsRead;
        var isEmptyLine = false;

        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < charsRead; i++)
            {
                if (buffer[i] == '\n')
                {
                    if (!isEmptyLine)
                    {
                        counter++;
                    }

                    isEmptyLine = true;
                }
                else if (!char.IsWhiteSpace(buffer[i]))
                {
                    isEmptyLine = false;
                }
            }
        }

        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();

        return counter - headerRows;
    }
}