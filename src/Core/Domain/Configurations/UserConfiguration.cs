using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Domain.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void EntityConfiguration(EntityTypeBuilder<User> builder)
        {
            builder.ToTable($"ms{nameof(User)}");
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Username).IsRequired();
            builder.Property(p => p.Password).IsRequired();
            builder.Property(p => p.Fullname).IsRequired();
            builder.Property(p => p.Email).IsRequired();
            builder.Property(p => p.Role).IsRequired();
        }
    }
}
