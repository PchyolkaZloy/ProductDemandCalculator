using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Sales.Application.Services.Config;
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
    private readonly IOptionsMonitor<AppSettingsConfig> _optionsMonitor;
    private readonly CancellationTokenSource _cts;
    private readonly SemaphoreSlim _semaphore;
    private int _currentParallelismDegree;

    public ProductProcessService(
        IFileReader fileReader,
        IFileWriter fileWriter,
        Channel<ProductInfo> channelProductInfo,
        Channel<ProductResult> channelProductResult,
        IProductProcessWorker productProcessWorker,
        IOptionsMonitor<AppSettingsConfig> optionsMonitor
    )
    {
        _fileReader = fileReader;
        _fileWriter = fileWriter;
        _channelProductInfo = channelProductInfo;
        _channelProductResult = channelProductResult;
        _productProcessWorker = productProcessWorker;
        _optionsMonitor = optionsMonitor;

        const int maxParallelismDegree = 100;
        _currentParallelismDegree = _optionsMonitor.CurrentValue.ParallelismDegree > maxParallelismDegree
            ? maxParallelismDegree
            : _optionsMonitor.CurrentValue.ParallelismDegree;
        _semaphore = new SemaphoreSlim(_currentParallelismDegree, maxParallelismDegree);
        _cts = new CancellationTokenSource();

        _optionsMonitor.OnChange(UpdateParallelismDegree);
    }


    public async Task StartProcessingAsync()
    {
        var readTask = _fileReader.ReadProductInfosAsync(_channelProductInfo.Writer, _cts.Token);
        var writeTask = _fileWriter.WriteProductResultsAsync(_channelProductResult.Reader, _cts.Token);

        var processingTask = Task.Run(async () =>
            {
                await foreach (var productInfo in _channelProductInfo.Reader.ReadAllAsync(_cts.Token))
                {
                    await _semaphore.WaitAsync(_cts.Token);

                    _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _productProcessWorker.ProcessAsync(productInfo, _cts.Token);
                            }
                            finally
                            {
                                _semaphore.Release();
                            }
                        },
                        _cts.Token
                    );
                }
            },
            _cts.Token
        );

        await processingTask;

        // Не костыль, а КОСТЫЛИЩЕ
        while (_semaphore.CurrentCount < _currentParallelismDegree)
        {
            await Task.Delay(50);
        }

        _channelProductResult.Writer.Complete();
        await Task.WhenAll(readTask, writeTask);
    }

    public void CancelProcessing()
    {
        _cts.Cancel();
    }

    private void UpdateParallelismDegree(AppSettingsConfig newSettingsConfig)
    {
        const int maxParallelismDegree = 100;

        var newParallelismDegree = newSettingsConfig.ParallelismDegree is < 1 or > maxParallelismDegree
            ? maxParallelismDegree
            : newSettingsConfig.ParallelismDegree;

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
}