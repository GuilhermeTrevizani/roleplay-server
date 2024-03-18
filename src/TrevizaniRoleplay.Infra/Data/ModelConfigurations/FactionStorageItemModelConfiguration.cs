using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class FactionStorageItemModelConfiguration : IEntityTypeConfiguration<FactionStorageItem>
    {
        public void Configure(EntityTypeBuilder<FactionStorageItem> builder)
        {
            builder.ToTable("FactionsStoragesItems");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.FactionStorage).WithMany().HasForeignKey(x => x.FactionStorageId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}