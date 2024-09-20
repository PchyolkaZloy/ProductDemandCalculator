using System.Threading.Channels;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Workers;

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

        await Task.Delay(10, cancellationToken);

        await channel.Writer.WriteAsync(productResult, cancellationToken);
    }
}