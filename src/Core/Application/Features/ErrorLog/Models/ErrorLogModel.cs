using MongoDB.Bson.Serialization.Attributes;

namespace Application.Features.ErrorLog.Models
{
    public class ErrorLogModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Message { get; set; } = string.Empty;
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDt { get; set; }
    }
}
