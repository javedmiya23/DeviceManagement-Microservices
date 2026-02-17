using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Api.Audit;

public class AuditLog
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string UserId { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
