using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthN.Persistence.EntityConfig
{
    /// <summary>
    /// Entity configuration for <see cref="AuthNRole"/>.
    /// </summary>
    public class RoleEntityConfig : IEntityTypeConfiguration<AuthNRole>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuthNRole> builder)
        {
            builder.Property<int>("RoleId").ValueGeneratedOnAdd();
            builder.HasKey("RoleId");

            builder.HasAlternateKey(r => r.Name);

            builder.Property(r => r.Name).HasMaxLength(30);
        }
    }
}
