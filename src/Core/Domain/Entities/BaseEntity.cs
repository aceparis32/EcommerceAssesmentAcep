namespace Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDt { get; set; }
    }
}
