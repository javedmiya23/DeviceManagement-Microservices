using System.Diagnostics;

namespace UserService.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var user = context.User?.Identity?.Name ?? "Anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Path} executed in {Elapsed} ms by {User}",
            context.Request.Method,
            context.Request.Path,
            stopwatch.ElapsedMilliseconds,
            user);
    }
}
