using MongoDB.Driver;

namespace DeviceService.Api.Audit;

public class MongoAuditRepository
{
    private readonly IMongoCollection<AuditLog> _collection;

    public MongoAuditRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoSettings:Database"]);

        _collection = database.GetCollection<AuditLog>("auditlog");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var index = Builders<AuditLog>.IndexKeys
            .Descending(x => x.CreatedAt);

        _collection.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(index));
    }

    public async Task BulkInsertAsync(List<AuditLog> logs)
    {
        if (logs.Count == 0)
            return;

        await _collection.InsertManyAsync(logs);
    }
}
