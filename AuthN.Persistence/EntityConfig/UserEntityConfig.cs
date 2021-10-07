using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Afi.Registration.Persistence.EntityConfig
{
    /// <summary>
    /// Entity configuration for <see cref="AuthNUser"/>.
    /// </summary>
    public class UserEntityConfig : IEntityTypeConfiguration<AuthNUser>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuthNUser> builder)
        {
            builder.HasIndex(r => r.Username).IsUnique();
            builder.HasIndex(r => r.RegisteredEmail).IsUnique();

            builder.Property<int>("UserId")
                .ValueGeneratedOnAdd()
                .HasAnnotation("Key", 0)
                .IsRequired();

            builder.Property(r => r.Username).IsRequired().HasMaxLength(50);
            builder.Property(r => r.RegisteredEmail).IsRequired()
                .HasMaxLength(512);
            builder.Property(r => r.PasswordSalt).IsRequired()
                .HasMaxLength(512);
            builder.Property(r => r.PasswordHash).IsRequired()
                .HasMaxLength(512);
            builder.Property(r => r.Forename).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Surname).IsRequired().HasMaxLength(50);

            // Many-to-many: an implicit "pure" join table is created
            builder.HasMany(r => r.Roles).WithMany(role => role.Users);
        }
    }
}
