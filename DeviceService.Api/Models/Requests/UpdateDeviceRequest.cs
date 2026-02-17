using System.ComponentModel.DataAnnotations;

namespace DeviceService.Api.Models.Requests;

public class UpdateDeviceRequest
{
    [Range(0, 100)]
    public int Battery { get; set; }

    public bool IsActive { get; set; }
}
