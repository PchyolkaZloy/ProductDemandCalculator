namespace Sales.Domain.Interfaces.Service;

public interface IProductProcessService
{
    Task StartProcessingAsync();

    void CancelProcessing();
}