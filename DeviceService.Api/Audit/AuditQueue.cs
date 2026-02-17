using System.Collections.Concurrent;

namespace DeviceService.Api.Audit;

public class AuditQueue : IAuditQueue
{
    private readonly ConcurrentQueue<AuditLog> _queue = new();

    public void Enqueue(AuditLog log)
    {
        _queue.Enqueue(log);
    }

    public bool TryDequeue(out AuditLog? log)
    {
        return _queue.TryDequeue(out log);
    }
}
