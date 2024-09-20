namespace Sales.Domain.Interfaces.Service;

public interface IProgressTracker
{
    long Read { get; }
    long Wrote { get; }
    long Completed { get; }
    long MaxValue { get; set; }

    void IncrementRead();
    void IncrementCompleted();
    void IncrementWritten();
}