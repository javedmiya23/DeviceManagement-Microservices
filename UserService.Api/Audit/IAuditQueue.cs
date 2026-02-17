namespace UserService.Api.Audit;

public interface IAuditQueue
{
    void Enqueue(AuditLog log);
    List<AuditLog> DequeueBatch(int batchSize);
}
