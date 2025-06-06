using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spoksy.Domain.Entities;

namespace Spoksy.Infrastructure.EntityMappings
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("messages");
            
            builder.HasKey(m => m.Id);

            builder.Property(m => m.ChatId)
                .IsRequired()
                .HasColumnName("chat_id");

            builder.Property(m => m.SenderId)
                .IsRequired()
                .HasColumnName("sender_id");

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(10000)
                .HasColumnName("content");

            builder.Property(m => m.SentAt)
                .IsRequired()
                .HasColumnName("sent_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(m => m.EditAt)
                .HasColumnName("edit_at");

            builder.Property(m => m.DeleteAt)
                .HasColumnName("delete_at");

            builder.Property(m => m.IsRead)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("is_read");

            builder.Property(m => m.IsEdit)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("is_edit");

            builder.Property(m => m.IsDelete)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("is_delete");

        }
    }
} 