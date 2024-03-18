using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class VehicleItemModelConfiguration : IEntityTypeConfiguration<VehicleItem>
    {
        public void Configure(EntityTypeBuilder<VehicleItem> builder)
        {
            builder.ToTable("VehiclesItems");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}