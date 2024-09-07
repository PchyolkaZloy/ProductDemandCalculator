
public class CurrentMiddleware : IMiddleware
{
    private readonly ILogger<CurrentMiddleware> _logger;
    private int _count;

    public CurrentMiddleware(ILogger<CurrentMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _count++;
        _logger.LogInformation("Current count: {0}", _count);
        await next(context);
    }
}