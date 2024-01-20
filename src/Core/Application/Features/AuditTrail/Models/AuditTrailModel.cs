using MongoDB.Bson.Serialization.Attributes;

namespace Application.Features.AuditTrail.Models
{
    public class AuditTrailModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Name")]
        public string? Name { get; set; } = string.Empty;
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDt { get; set; }
    }
}
