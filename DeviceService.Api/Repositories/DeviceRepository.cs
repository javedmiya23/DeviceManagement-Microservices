using MongoDB.Driver;
using DeviceService.Api.Models.Entities;

namespace DeviceService.Api.Repositories;

public class DeviceRepository
{
    private readonly IMongoCollection<Device> _collection;

    public DeviceRepository(IConfiguration config)
    {
        var connectionString = config["MongoSettings:ConnectionString"];
        var databaseName = config["MongoSettings:Database"];
        var collectionName = config["MongoSettings:Collection"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _collection = database.GetCollection<Device>(collectionName);
    }

    public async Task AddAsync(Device device)
    {
        await _collection.InsertOneAsync(device);
    }

    public async Task<Device?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Device>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task UpdateAsync(Device device)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == device.Id,
            device);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
