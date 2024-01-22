namespace AssesmentAcep.WebAPI.Models
{
    public class MongoDbSettingsModel
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string AuditTrailCollectionName { get; set; } = null!;
        public string ErrorLogCollectionName { get; set; } = null!;
    }
}
