using System.Threading.Channels;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Services;

public sealed class ProductProcessWorker(
    IProductCalculator calculator,
    IProgressTracker progressTracker,
    Channel<ProductResult> channel
)
    : IProductProcessWorker
{
    public async Task ProcessAsync(ProductInfo productInfo, CancellationToken cancellationToken)
    {
        var productResult = calculator.CalculateDemand(productInfo);
        progressTracker.IncrementCompleted();

        // Задержка для имитации длительных вычислений
        await Task.Delay(1, cancellationToken);
        
        await channel.Writer.WriteAsync(productResult, cancellationToken);
    }
}