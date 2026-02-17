using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceService.Api.Models.Entities;

public class Device
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string MAC { get; set; } = default!;
    public string IMEI { get; set; } = default!;
    public string IMSI { get; set; } = default!;
    public int Battery { get; set; }
    public string PlatformType { get; set; } = default!;
    public DateTime RegisteredAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
