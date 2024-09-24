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

        int productInfoSizeInBytes;
        int productResultSizeInBytes;
        if (IntPtr.Size == 4)
        {
            // 32-bit
            productInfoSizeInBytes = 32;   // obj header + 3 longs (8 + 3 * 8)
            productResultSizeInBytes = 24; // obj header + 2 longs (8 + 2 * 8)
        }
        else
        {
            // 64-bit
            productInfoSizeInBytes = 40;   // obj header + 3 longs (16 + 3 * 8)
            productResultSizeInBytes = 32; // obj header + 2 longs (16 + 2 * 8)
        }

        var channelReaderCapacity = channelReaderCapacityInMb * 1024 * 1024 / productInfoSizeInBytes;
        collection.AddSingleton(Channel.CreateBounded<ProductInfo>(
            new BoundedChannelOptions(capacity: channelReaderCapacity)
            {
                SingleWriter = true
            }));

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