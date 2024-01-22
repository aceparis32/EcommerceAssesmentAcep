using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserTransaction : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}
