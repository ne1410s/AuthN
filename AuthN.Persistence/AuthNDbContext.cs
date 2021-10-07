using Afi.Registration.Persistence.EntityConfig;
using AuthN.Domain.Models.Storage;
using Microsoft.EntityFrameworkCore;

namespace AuthN.Persistence
{
    /// <summary>
    /// Data context for the AuthN database.
    /// </summary>
    public class AuthNDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthNDbContext"/>
        /// class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AuthNDbContext(DbContextOptions<AuthNDbContext> options)
            : base(options)
        { }

        /// <summary>
        /// Gets or sets the customers table.
        /// </summary>
        public DbSet<AuthNUser> Users { get; init; } = default!;

        /// <summary>
        /// Gets or sets the policies table.
        /// </summary>
        public DbSet<AuthNRole> Roles { get; init; } = default!;

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserEntityConfig());
            modelBuilder.ApplyConfiguration(new RoleEntityConfig());
        }
    }
}
