using System.Diagnostics;

namespace DeviceService.Api.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        MongoLogRepository repository)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var log = new AppLog
        {
            TraceId = context.TraceIdentifier,
            Method = context.Request.Method,
            Path = context.Request.Path,
            StatusCode = context.Response.StatusCode,
            TimeTakenMs = stopwatch.ElapsedMilliseconds,
            Username = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : null,
            CreatedAt = DateTime.UtcNow
        };

        await repository.InsertAsync(log);
    }
}
