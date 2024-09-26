namespace Sales.Domain.Interfaces.Service;

public interface IProductProcessService
{
    Task StartProcessingAsync(int parallelismDegree, CancellationToken cancellationToken);

    void UpdateParallelismDegree(int newParallelismDegree);
}