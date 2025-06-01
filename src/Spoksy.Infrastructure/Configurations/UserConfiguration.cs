using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("email");

            builder.Property(u => u.BirthDate)
                .IsRequired()
                .HasColumnName("birth_date");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.LastActiveAt)
                .HasColumnName("last_active_at");

            builder.Property(u => u.CurrentCountry)
               .IsRequired()
               .HasColumnName("current_country")
               .HasMaxLength(4)
               .HasConversion(
                   v => v.Code, 
                   v => Country.GetByCode(v) 
               );

            builder.HasMany<UserLanguage>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId);

            builder.HasMany<ChatParticipant>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId);
        }
    }
} 