using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        #region DbSet Only
        DbSet<User> Users { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<UserTransaction> UserTransactions { get; set; }

        #endregion

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
