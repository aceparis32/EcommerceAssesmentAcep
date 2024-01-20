using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations
{
    public class UserTransactionConfiguration : BaseEntityConfiguration<UserTransaction>
    {
        public override void EntityConfiguration(EntityTypeBuilder<UserTransaction> builder)
        {
            builder.ToTable($"tr{nameof(UserTransaction)}");
            builder.HasKey(x => x.Id);
        }
    }
}
