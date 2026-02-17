using MongoDB.Driver;
using UserService.Api.Audit;

namespace UserService.Api.Audit;

public class MongoAuditRepository
{
    private readonly IMongoCollection<AuditLog> _collection;

    public MongoAuditRepository(IConfiguration config)
    {
        var connectionString = config["MongoSettings:ConnectionString"];
        var databaseName = config["MongoSettings:Database"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _collection = database.GetCollection<AuditLog>("auditlog");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<AuditLog>.IndexKeys
            .Descending(x => x.CreatedAt);

        _collection.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(indexKeys));
    }

    public async Task BulkInsertAsync(List<AuditLog> logs)
    {
        if (logs.Count == 0)
            return;

        await _collection.InsertManyAsync(logs);
    }
}
