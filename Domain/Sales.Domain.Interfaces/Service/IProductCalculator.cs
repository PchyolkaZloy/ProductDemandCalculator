using Sales.Domain.Models.Products;

namespace Sales.Domain.Interfaces.Service;

public interface IProductCalculator
{
    ProductResult CalculateDemand(ProductInfo productInfo);
}