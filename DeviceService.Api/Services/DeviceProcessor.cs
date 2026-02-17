using System.Text.Json;
using DeviceService.Api.Models.Entities;
using DeviceService.Api.Models.Requests;
using DeviceService.Api.Repositories;
using DeviceService.Api.Caches.Interfaces;
using DeviceService.Api.Audit;
namespace DeviceService.Api.Services;

public class DeviceProcessor
{
    private readonly DeviceRepository _repository;
    private readonly IDeviceCache _cache;

    private readonly IAuditQueue _auditQueue;

    public DeviceProcessor(
        DeviceRepository repository,
        IDeviceCache cache,
        IAuditQueue auditQueue)
    {
        _repository = repository;
        _cache = cache;
        _auditQueue = auditQueue;
    }


    
    public async Task HandleCreateAsync(string payload)
    {
        var request = JsonSerializer.Deserialize<CreateDeviceRequest>(payload)!;

        var device = new Device
        {
            Id = Guid.NewGuid().ToString(),
            MAC = request.MAC,
            IMEI = request.IMEI,
            IMSI = request.IMSI,
            Battery = request.Battery,
            PlatformType = request.PlatformType,
            RegisteredAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(device);
        await _cache.AddAsync(device);

            _auditQueue.Enqueue(new AuditLog
            {
                DeviceId = device.Id,
                Message = $"Device {device.Id} created.",
                CreatedAt = DateTime.UtcNow
            });

    }

    public async Task HandleUpdateAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(payload)!;

        var id = data["Id"].ToString();
        var battery = Convert.ToInt32(data["Battery"]);
        var isActive = Convert.ToBoolean(data["IsActive"]);

        var device = await _repository.GetByIdAsync(id!);
        if (device == null)
            return;

        device.Battery = battery;
        device.IsActive = isActive;
        device.LastUpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(device);
        await _cache.UpdateAsync(device);

        _auditQueue.Enqueue(new AuditLog
        {
            DeviceId = device.Id,
            Message = $"Device {device.Id} updated.",
            CreatedAt = DateTime.UtcNow
        });

    }

   
  
    public async Task HandleDeleteAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(payload)!;

        var id = data["Id"];

        await _repository.DeleteAsync(id);
        await _cache.DeleteAsync(id);

        _auditQueue.Enqueue(new AuditLog
        {
            DeviceId = id,
            Message = $"Device {id} deleted.",
            CreatedAt = DateTime.UtcNow
        });

    }
}
