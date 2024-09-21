using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Services;

public sealed class SimpleProductCalculator : IProductCalculator
{
    public ProductResult CalculateDemand(ProductInfo productInfo)
    {
        return new ProductResult(productInfo.Id, Math.Max(productInfo.Prediction - productInfo.Stock, 0));
    }
}