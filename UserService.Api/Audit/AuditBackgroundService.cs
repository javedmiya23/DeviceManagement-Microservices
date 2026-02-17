using Microsoft.Extensions.Hosting;

namespace UserService.Api.Audit;

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
        while (!stoppingToken.IsCancellationRequested)
        {
            var batch = _queue.DequeueBatch(50);

            if (batch.Count > 0)
            {
                await _repository.BulkInsertAsync(batch);
                _logger.LogInformation("Inserted {Count} audit logs", batch.Count);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
