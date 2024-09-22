using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Services;

public sealed class SimpleProductCalculator : IProductCalculator
{
    public ProductResult CalculateDemand(ProductInfo productInfo)
    {
        // Цикл для имитации длительных вычислений
        for (var i = 0; i < 100_000; ++i)
        {
        }

        return new ProductResult(productInfo.Id, Math.Max(productInfo.Prediction - productInfo.Stock, 0));
    }
}