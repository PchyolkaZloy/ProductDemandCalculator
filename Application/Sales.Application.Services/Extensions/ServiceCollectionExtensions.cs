using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Services.Services;
using Sales.Application.Services.Trackers;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection collection,
        int channelReaderCapacityInMb,
        int channelWriterCapacityInMb
    )
    {
        collection.AddTransient<IProductCalculator, SimpleProductCalculator>();
        collection.AddTransient<IProductProcessWorker, ProductProcessWorker>();

        const int productInfoSizeInBytes = 24;
        var channelReaderCapacity = channelReaderCapacityInMb * 1024 * 1024 / productInfoSizeInBytes;
        collection.AddSingleton(Channel.CreateBounded<ProductInfo>(
            new BoundedChannelOptions(capacity: channelReaderCapacity)
            {
                SingleWriter = true
            }));

        const int productResultSizeInBytes = 16;
        var channelWriterCapacity = channelWriterCapacityInMb * 1024 * 1024 / productResultSizeInBytes;
        collection.AddSingleton(Channel.CreateBounded<ProductResult>(
            new BoundedChannelOptions(capacity: channelWriterCapacity)
            {
                SingleReader = true,
            }));

        collection.AddSingleton<IProgressTracker, ProgressTracker>();
        collection.AddSingleton<IProductProcessService, ProductProcessService>();

        return collection;
    }
}