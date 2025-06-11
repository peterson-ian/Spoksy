using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spoksy.Domain.Entities;

namespace Spoksy.Infrastructure.EntityMappings
{
    public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.ToTable("chat_participants"); 
            
            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            builder.Property(cp => cp.ChatId)
                .IsRequired()
                .HasColumnName("chat_id");

            builder.Property(cp => cp.JoinAt)
                .IsRequired()
                .HasColumnName("join_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(cp => cp.LeaveAt)
                .HasColumnName("leave_at");

            builder.HasIndex(cp => new { cp.ChatId, cp.UserId })
                .IsUnique();

            builder.HasOne<User>()
             .WithMany()
             .HasForeignKey(ul => ul.UserId);

            builder.HasOne<Chat>()
             .WithMany()
             .HasForeignKey(ul => ul.ChatId);
        }
    }
} 