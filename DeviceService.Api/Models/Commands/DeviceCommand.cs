namespace DeviceService.Api.Models.Commands;

public class DeviceCommand
{
    public string CommandType { get; set; } = default!;
    public string CorrelationId { get; set; } = default!;
    public string Payload { get; set; } = default!;
}
