using System.Threading.Channels;
using Sales.Domain.Models.Products;

namespace Sales.Domain.Interfaces.FileIO;

public interface IFileWriter
{
    Task WriteProductResultsAsync(ChannelReader<ProductResult> channelReader, CancellationToken cancellationToken);
}