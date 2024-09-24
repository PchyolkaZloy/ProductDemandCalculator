using System.Threading.Channels;
using Sales.Domain.Interfaces.FileIO;
using Sales.Domain.Interfaces.Service;
using Sales.Domain.Models.Products;

namespace Sales.Application.Services.Services;

public sealed class ProductProcessService : IProductProcessService
{
    private readonly IFileReader _fileReader;
    private readonly IFileWriter _fileWriter;
    private readonly Channel<ProductInfo> _channelProductInfo;
    private readonly Channel<ProductResult> _channelProductResult;
    private readonly IProductProcessWorker _productProcessWorker;
    private SemaphoreSlim _semaphore;
    private int _currentParallelismDegree;
    private const int MaxParallelismDegree = 100;

    public ProductProcessService(
        IFileReader fileReader,
        IFileWriter fileWriter,
        Channel<ProductInfo> channelProductInfo,
        Channel<ProductResult> channelProductResult,
        IProductProcessWorker productProcessWorker
    )
    {
        _fileReader = fileReader;
        _fileWriter = fileWriter;
        _channelProductInfo = channelProductInfo;
        _channelProductResult = channelProductResult;
        _productProcessWorker = productProcessWorker;
    }


    public async Task StartProcessingAsync(int parallelismDegree, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _currentParallelismDegree = parallelismDegree;
        _semaphore = new SemaphoreSlim(_currentParallelismDegree, MaxParallelismDegree);

        var readTask = _fileReader.ReadProductInfosAsync(_channelProductInfo.Writer, cancellationToken);
        var writeTask = _fileWriter.WriteProductResultsAsync(_channelProductResult.Reader, cancellationToken);

        var processingTask = Task.Run(async () =>
            {
                await foreach (var productInfo in _channelProductInfo.Reader.ReadAllAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _semaphore.WaitAsync(cancellationToken);

                    _ = CalculateProcessAsync(_productProcessWorker, productInfo, _semaphore, cancellationToken);
                }
            },
            cancellationToken
        );

        await processingTask;

        // Дожидаемся, пока внутренние задачи processingTask закончатся
        while (_semaphore.CurrentCount < _currentParallelismDegree)
        {
            await Task.Delay(50, cancellationToken);
        }

        _channelProductResult.Writer.Complete();
        await Task.WhenAll(readTask, writeTask);
    }

    public void UpdateParallelismDegree(int newParallelismDegree)
    {
        if (newParallelismDegree is < 1 or > MaxParallelismDegree)
        {
            newParallelismDegree = MaxParallelismDegree;
        }

        if (newParallelismDegree > _currentParallelismDegree)
        {
            _semaphore.Release(newParallelismDegree - _currentParallelismDegree);
        }
        else
        {
            for (var i = 0; i < _currentParallelismDegree - newParallelismDegree; i++)
            {
                _semaphore.Wait();
            }
        }

        _currentParallelismDegree = newParallelismDegree;
    }


    private static async Task CalculateProcessAsync(
        IProductProcessWorker productProcessWorker,
        ProductInfo productInfo,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await productProcessWorker.ProcessAsync(productInfo, cancellationToken);
            await Task.Delay(10, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }
}