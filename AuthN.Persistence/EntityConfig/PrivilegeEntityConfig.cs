using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthN.Persistence.EntityConfig
{
    /// <summary>
    /// Entity configuration for <see cref="AuthNPrivilege"/>.
    /// </summary>
    public class PrivilegeEntityConfig
        : IEntityTypeConfiguration<AuthNPrivilege>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuthNPrivilege> builder)
        {
            builder.Property<int>("PrivilegeId").ValueGeneratedNever();
            builder.HasKey("PrivilegeId");

            builder.HasAlternateKey(r => r.Type);

            builder.Property(r => r.Type)
                .HasConversion(new EnumStringConverter<PrivilegeType>())
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
