using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class ConfiscationItemModelConfiguration : IEntityTypeConfiguration<ConfiscationItem>
    {
        public void Configure(EntityTypeBuilder<ConfiscationItem> builder)
        {
            builder.ToTable("ConfiscationsItems");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Confiscation).WithMany(x => x.Items).HasForeignKey(x => x.ConfiscationId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}