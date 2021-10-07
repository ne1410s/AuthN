using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Afi.Registration.Persistence.EntityConfig
{
    /// <summary>
    /// Entity configuration for <see cref="AuthNRole"/>.
    /// </summary>
    public class RoleEntityConfig : IEntityTypeConfiguration<AuthNRole>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuthNRole> builder)
        {
            builder.HasAlternateKey(r => r.Name);

            builder.Property<int>("RoleId")
                .ValueGeneratedOnAdd()
                .HasAnnotation("Key", 0)
                .IsRequired();

            builder.Property(r => r.Name).HasMaxLength(30);
        }
    }
}
