using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceService.Api.Logging;

public class AppLog
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string TraceId { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string Path { get; set; } = default!;
    public int StatusCode { get; set; }
    public long TimeTakenMs { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }
}
