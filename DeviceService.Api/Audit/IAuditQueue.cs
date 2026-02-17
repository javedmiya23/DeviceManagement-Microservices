namespace DeviceService.Api.Audit;

public interface IAuditQueue
{
    void Enqueue(AuditLog log);
    bool TryDequeue(out AuditLog? log);
}
