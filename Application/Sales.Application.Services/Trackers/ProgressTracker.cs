using Sales.Domain.Interfaces.Service;

namespace Sales.Application.Services.Trackers;

public sealed class ProgressTracker : IProgressTracker
{
    private long _read;
    private long _written;
    private long _completed;


    public long Read => _read;
    public long Wrote => _written;
    public long Completed => _completed;
    public long MaxValue { get; set; }

    public void IncrementRead()
    {
        Interlocked.Increment(ref _read);
    }

    public void IncrementCompleted()
    {
        Interlocked.Increment(ref _completed);
    }

    public void IncrementWritten()
    {
        Interlocked.Increment(ref _written);
    }
}