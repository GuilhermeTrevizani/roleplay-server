﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class ConfiscationModelConfiguration : IEntityTypeConfiguration<Confiscation>
    {
        public void Configure(EntityTypeBuilder<Confiscation> builder)
        {
            builder.ToTable("Confiscations");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PoliceOfficerCharacter).WithMany().HasForeignKey(x => x.PoliceOfficerCharacterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Faction).WithMany().HasForeignKey(x => x.FactionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}