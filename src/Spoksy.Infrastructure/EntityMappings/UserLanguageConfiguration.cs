using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Configurations
{
    public class UserLanguageConfiguration : IEntityTypeConfiguration<UserLanguage>
    {
        public void Configure(EntityTypeBuilder<UserLanguage> builder)
        {
            builder.ToTable("user_languages");
            
            builder.HasKey(ul => ul.Id);

            builder.Property(ul => ul.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            builder.Property(ul => ul.StartedLearningOn)
                .IsRequired()
                .HasColumnName("started_learning_on")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(ul => ul.ProficiencyLevel)
                .IsRequired()
                .HasColumnName("proficiency_level")
                .HasConversion<string>();

            builder.Property(ul => ul.Language)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("language")
                .HasConversion(
                   v => v.Code,
                   v => Language.GetByCode(v)
                );

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(ul => ul.UserId);

        }
    }
} 