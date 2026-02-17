using System.Collections.Concurrent;

namespace UserService.Api.Audit;

public class AuditQueue : IAuditQueue
{
    private readonly ConcurrentQueue<AuditLog> _queue =
        new ConcurrentQueue<AuditLog>();

    public void Enqueue(AuditLog log)
    {
        _queue.Enqueue(log);
    }

    public List<AuditLog> DequeueBatch(int batchSize)
    {
        var list = new List<AuditLog>();

        while (list.Count < batchSize && _queue.TryDequeue(out var log))
        {
            list.Add(log);
        }

        return list;
    }
}
