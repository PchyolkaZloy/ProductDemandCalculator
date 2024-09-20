using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Services.Calculators;
using Sales.Application.Services.Services;
using Sales.Application.Services.Trackers;
using Sales.Application.Services.Workers;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
    {
        collection.AddTransient<IProductCalculator, SimpleProductCalculator>();
        collection.AddTransient<IProductProcessWorker, ProductProcessWorker>();

        collection.AddSingleton(Channel.CreateUnbounded<ProductInfo>());
        collection.AddSingleton(Channel.CreateUnbounded<ProductResult>());

        collection.AddSingleton<IProgressTracker, ProgressTracker>();
        collection.AddSingleton<IProductProcessService, ProductProcessService>();

        return collection;
    }
}