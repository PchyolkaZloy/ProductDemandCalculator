using Sales.Domain.Models.Products;

namespace Sales.Domain.Interfaces.Service;

public interface IProductProcessWorker
{
    Task ProcessAsync(ProductInfo productInfo, CancellationToken cancellationToken);
}