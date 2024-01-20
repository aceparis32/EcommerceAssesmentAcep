using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Configurations
{
    public abstract class BaseEntityConfiguration<TBaseEntity> : IEntityTypeConfiguration<TBaseEntity> where TBaseEntity : BaseEntity
    {
        public void Configure(EntityTypeBuilder<TBaseEntity> builder)
        {
            builder.Property(p => p.CreatedBy).IsRequired();
            builder.Property(p => p.CreatedDt).IsRequired();

            EntityConfiguration(builder);
        }

        public abstract void EntityConfiguration(EntityTypeBuilder<TBaseEntity> builder);
    }
}
