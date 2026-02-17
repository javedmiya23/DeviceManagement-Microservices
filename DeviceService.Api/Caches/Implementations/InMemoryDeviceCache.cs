using System.Collections.Concurrent;
using DeviceService.Api.Caches.Interfaces;
using DeviceService.Api.Models.Entities;

namespace DeviceService.Api.Caches.Implementations;

public class InMemoryDeviceCache : IDeviceCache
{
    private readonly ConcurrentDictionary<string, Device> _cache
        = new ConcurrentDictionary<string, Device>();

    public Task<Device?> GetAsync(string deviceId)
    {
        _cache.TryGetValue(deviceId, out var device);
        return Task.FromResult(device);
    }

    public Task<List<Device>> GetAllAsync()
    {
        return Task.FromResult(_cache.Values.ToList());
    }

    public Task AddAsync(Device device)
    {
        _cache[device.Id] = device;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Device device)
    {
        _cache[device.Id] = device;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string deviceId)
    {
        _cache.TryRemove(deviceId, out _);
        return Task.CompletedTask;
    }
}
