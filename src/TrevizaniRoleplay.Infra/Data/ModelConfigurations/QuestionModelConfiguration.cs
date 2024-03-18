using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrevizaniRoleplay.Domain.Entities;

namespace TrevizaniRoleplay.Infra.Data.ModelConfigurations
{
    public class QuestionModelConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(1000);
            builder.HasOne(x => x.CorrectQuestionAnswer).WithMany().HasForeignKey(x => x.CorrectQuestionAnswerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}