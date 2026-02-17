using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DeviceService.Api.Infrastructure.Kafka;
using DeviceService.Api.Models.Commands;
using DeviceService.Api.Models.Requests;
using DeviceService.Api.Attributes;
using DeviceService.Api.Repositories;
using DeviceService.Api.Caches.Interfaces;

namespace DeviceService.Api.Controllers;

[ApiController]
[Route("api/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly KafkaProducer _producer;
    private readonly DeviceRepository _repository;
    private readonly IDeviceCache _cache;

    public DevicesController(
        KafkaProducer producer,
        DeviceRepository repository,
        IDeviceCache cache)
    {
        _producer = producer;
        _repository = repository;
        _cache = cache;
    }

    [RequirePermission(4)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request)
    {
        var command = new DeviceCommand
        {
            CommandType = "CreateDevice",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(request)
        };

        await _producer.ProduceAsync("device-commands", command);

        return Accepted(new { command.CorrelationId });
    }


    [RequirePermission(2)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateDeviceRequest request)
    {
        var command = new DeviceCommand
        {
            CommandType = "UpdateDevice",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(new
            {
                Id = id,
                request.Battery,
                request.IsActive
            })
        };

        await _producer.ProduceAsync("device-commands", command);

        return Accepted(new { command.CorrelationId });
    }

    
    [RequirePermission(3)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new DeviceCommand
        {
            CommandType = "DeleteDevice",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(new { Id = id })
        };

        await _producer.ProduceAsync("device-commands", command);

        return Accepted(new { command.CorrelationId });
    }

    [RequirePermission(1)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var device = await _cache.GetAsync(id);

        if (device == null)
        {
            device = await _repository.GetByIdAsync(id);

            if (device != null)
                await _cache.AddAsync(device);
        }

        if (device == null)
            return NotFound();

        return Ok(device);
    }

    
    [RequirePermission(1)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _cache.GetAllAsync();

        if (devices.Count == 0)
        {
            devices = await _repository.GetAllAsync();

            foreach (var device in devices)
                await _cache.AddAsync(device);
        }

        return Ok(devices);
    }
}
