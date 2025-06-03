using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("chats");
            
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnName("created_At")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.LastActivityAt)
                .IsRequired()
                .HasColumnName("last_active_at");

            builder.Property(c => c.Status)
                .IsRequired()
                .HasColumnName("status")
                .HasConversion(new EnumToStringConverter<ChatStatus>());

            builder.Property(c => c.PrimaryLanguage)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("primary_language")
                .HasConversion(
                   c => c.Code,
                   c => Language.GetByCode(c)
                ); ;

            builder.Property(c => c.SecondaryLanguage)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("secondary_language")
                .HasConversion(
                   c => c.Code,
                   c => Language.GetByCode(c)
                );

            builder.Property(c => c.MaxParticipants)
                .IsRequired()
                .HasDefaultValue(2);

            builder.HasMany<ChatParticipant>()
                .WithOne()
                .HasForeignKey(cp => cp.ChatId);

        }
    }
} 