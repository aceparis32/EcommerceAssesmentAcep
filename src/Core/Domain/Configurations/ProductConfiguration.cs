using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void EntityConfiguration(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable($"ms{nameof(Product)}");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name);
        }
    }
}
