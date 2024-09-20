using System.Threading.Channels;
using Sales.Domain.Models.Products;

namespace Sales.Domain.Interfaces.FileIO;

public interface IFileReader
{
    Task ReadProductInfosAsync(ChannelWriter<ProductInfo> channelWriter, CancellationToken cancellationToken);
}