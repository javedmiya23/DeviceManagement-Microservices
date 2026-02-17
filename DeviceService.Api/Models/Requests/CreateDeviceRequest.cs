using System.ComponentModel.DataAnnotations;

namespace DeviceService.Api.Models.Requests;

public class CreateDeviceRequest
{
    [Required]
    [RegularExpression(@"^([0-9A-Fa-f]{2}:){5}([0-9A-Fa-f]{2})$",
        ErrorMessage = "Invalid MAC address format.")]
    public string MAC { get; set; } = default!;

    [Required]
    [RegularExpression(@"^\d{15}$",
        ErrorMessage = "IMEI must be exactly 15 digits.")]
    public string IMEI { get; set; } = default!;

    [Required]
    [RegularExpression(@"^\d{15}$",
        ErrorMessage = "IMSI must be exactly 15 digits.")]
    public string IMSI { get; set; } = default!;

    [Range(0, 100)]
    public int Battery { get; set; }

    [Required]
    [RegularExpression("^(Android|iOS)$",
        ErrorMessage = "PlatformType must be Android or iOS.")]
    public string PlatformType { get; set; } = default!;
}
