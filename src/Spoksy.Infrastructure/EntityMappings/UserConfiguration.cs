using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.EntityMappings
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

            builder.Property(u => u.LastActivityAt)
                .HasColumnName("last_active_at");

            builder.Property(u => u.CurrentCountry)
               .IsRequired()
               .HasColumnName("current_country")
               .HasMaxLength(4)
               .HasConversion(
                   v => v.Code, 
                   v => Country.GetByCode(v) 
               );

            builder.Property(u => u.IdentityProviderId)
                .HasColumnName("identity_provider_id");

            builder.Property(c => c.Status)
               .IsRequired()
               .HasColumnName("status")
               .HasConversion(new EnumToStringConverter<UserStatus>());

            builder.HasMany<UserLanguage>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId);

            builder.HasMany<ChatParticipant>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId);
        }
    }
} 