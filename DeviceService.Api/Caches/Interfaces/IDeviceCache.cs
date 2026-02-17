using DeviceService.Api.Models.Entities;

namespace DeviceService.Api.Caches.Interfaces;

public interface IDeviceCache
{
    Task<Device?> GetAsync(string deviceId);
    Task<List<Device>> GetAllAsync();
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(string deviceId);
}
