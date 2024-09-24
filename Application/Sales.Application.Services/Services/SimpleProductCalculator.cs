using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Services;

public sealed class SimpleProductCalculator : IProductCalculator
{
    public ProductResult CalculateDemand(ProductInfo productInfo)
    {
        // Длительныe вычисления
        double sum = 0;
        for (var i = 0; i < 1_000_000; ++i)
        {
            sum += Math.Sqrt(productInfo.Id);
        }

        return new ProductResult(productInfo.Id, Math.Max(productInfo.Prediction - productInfo.Stock, 0));
    }
}