using StackExchange.Redis;
using DeviceService.Api.Caches.Interfaces;
using DeviceService.Api.Models.Entities;

namespace DeviceService.Api.Caches.Implementations;

public class RedisDeviceCache : IDeviceCache
{
    private readonly IDatabase _database;

    public RedisDeviceCache(IConfiguration config)
    {
        var connection = ConnectionMultiplexer.Connect(
            config["Redis:ConnectionString"]!);

        _database = connection.GetDatabase();
    }

    private string GetKey(string id) => $"device:{id}";

    public async Task<Device?> GetAsync(string deviceId)
    {
        var key = GetKey(deviceId);

        if (!await _database.KeyExistsAsync(key))
            return null;

        var entries = await _database.HashGetAllAsync(key);

        if (entries.Length == 0)
            return null;

        return new Device
        {
            Id = deviceId,
            MAC = entries.First(e => e.Name == "MAC").Value!,
            IMEI = entries.First(e => e.Name == "IMEI").Value!,
            IMSI = entries.First(e => e.Name == "IMSI").Value!,
            Battery = (int)entries.First(e => e.Name == "Battery").Value,
            PlatformType = entries.First(e => e.Name == "PlatformType").Value!,
            RegisteredAt = DateTime.Parse(entries.First(e => e.Name == "RegisteredAt").Value!),
            LastUpdatedAt = DateTime.Parse(entries.First(e => e.Name == "LastUpdatedAt").Value!),
            IsActive = (bool)entries.First(e => e.Name == "IsActive").Value
        };
    }

    public async Task<List<Device>> GetAllAsync()
    {
        var server = _database.Multiplexer.GetServer(
            _database.Multiplexer.GetEndPoints().First());

        var keys = server.Keys(pattern: "device:*");

        var devices = new List<Device>();

        foreach (var key in keys)
        {
            var id = key.ToString().Replace("device:", "");
            var device = await GetAsync(id);
            if (device != null)
                devices.Add(device);
        }

        return devices;
    }

    public async Task AddAsync(Device device)
    {
        var key = GetKey(device.Id);

        await _database.HashSetAsync(key, new HashEntry[]
        {
            new("MAC", device.MAC),
            new("IMEI", device.IMEI),
            new("IMSI", device.IMSI),
            new("Battery", device.Battery),
            new("PlatformType", device.PlatformType),
            new("RegisteredAt", device.RegisteredAt.ToString("O")),
            new("LastUpdatedAt", device.LastUpdatedAt.ToString("O")),
            new("IsActive", device.IsActive)
        });
    }

    public async Task UpdateAsync(Device device)
    {
        await AddAsync(device);
    }

    public async Task DeleteAsync(string deviceId)
    {
        await _database.KeyDeleteAsync(GetKey(deviceId));
    }
}
