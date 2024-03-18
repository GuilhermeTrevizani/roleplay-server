﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class CharacterItemModelConfiguration : IEntityTypeConfiguration<CharacterItem>
    {
        public void Configure(EntityTypeBuilder<CharacterItem> builder)
        {
            builder.ToTable("CharactersItems");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}