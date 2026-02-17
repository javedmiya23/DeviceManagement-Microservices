using MongoDB.Driver;

namespace DeviceService.Api.Logging;

public class MongoLogRepository
{
    private readonly IMongoCollection<AppLog> _collection;

    public MongoLogRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoSettings:Database"]);

        _collection = database.GetCollection<AppLog>("AppLogs");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var index = Builders<AppLog>.IndexKeys
            .Descending(x => x.CreatedAt);

        _collection.Indexes.CreateOne(
            new CreateIndexModel<AppLog>(index));
    }

    public async Task InsertAsync(AppLog log)
    {
        await _collection.InsertOneAsync(log);
    }
}
