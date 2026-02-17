using Microsoft.Extensions.Hosting;

namespace DeviceService.Api.Audit;

public class AuditBackgroundService : BackgroundService
{
    private readonly IAuditQueue _queue;
    private readonly MongoAuditRepository _repository;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(
        IAuditQueue queue,
        MongoAuditRepository repository,
        ILogger<AuditBackgroundService> logger)
    {
        _queue = queue;
        _repository = repository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<AuditLog>();

        while (!stoppingToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var log))
            {
                buffer.Add(log!);

                if (buffer.Count >= 20)
                {
                    await FlushAsync(buffer);
                }
            }

            if (buffer.Count > 0)
            {
                await FlushAsync(buffer);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task FlushAsync(List<AuditLog> buffer)
    {
        try
        {
            await _repository.BulkInsertAsync(buffer);
            buffer.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert audit logs");
        }
    }
}
