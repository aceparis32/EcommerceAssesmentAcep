using Domain.Enums;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public RoleEnum Role { get; set; }

        public virtual ICollection<UserTransaction> UserTransactions { get; set; }
    }
}
