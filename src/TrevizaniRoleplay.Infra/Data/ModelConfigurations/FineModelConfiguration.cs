using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class FineModelConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.ToTable("Fines");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Reason).HasMaxLength(255);
            builder.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PoliceOfficerCharacter).WithMany().HasForeignKey(x => x.PoliceOfficerCharacterId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}