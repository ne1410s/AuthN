using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthN.Persistence.EntityConfig
{
    /// <summary>
    /// Entity configuration for <see cref="AuthNUser"/>.
    /// </summary>
    public class UserEntityConfig : IEntityTypeConfiguration<AuthNUser>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuthNUser> builder)
        {
            builder.Property<int>("UserId").ValueGeneratedOnAdd();
            builder.HasKey("UserId");

            builder.HasIndex(r => r.Username).IsUnique();
            builder.HasIndex(r => r.RegisteredEmail).IsUnique();

            builder.Property(r => r.Username).HasMaxLength(50);
            builder.Property(r => r.RegisteredEmail).IsRequired()
                .HasMaxLength(512);
            builder.Property(r => r.PasswordSalt).HasMaxLength(512);
            builder.Property(r => r.PasswordHash).HasMaxLength(512);
            builder.Property(r => r.Forename).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Surname).IsRequired().HasMaxLength(50);
            builder.Property(r => r.FacebookId).HasMaxLength(50);

            // Many-to-many: an implicit "pure" join table is created
            builder.HasMany(r => r.Privileges).WithMany(p => p.Users);
        }
    }
}
