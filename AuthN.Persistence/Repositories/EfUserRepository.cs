using System;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using AuthN.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Afi.Registration.Persistence.Repositories
{
    /// <inheritdoc cref="IUserRepository"/>
    public class EfUserRepository : IUserRepository
    {
        private readonly AuthNDbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUserRepository"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public EfUserRepository(AuthNDbContext db)
        {
            this.db = db;
        }

        /// <inheritdoc/>
        public async Task ActivateAsync(string username)
        {
            var user = await FindByUsernameAsync(username)
                ?? throw new DataStateException("User not found");

            user.ActivatedOn ??= DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(AuthNUser user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<AuthNUser?> FindByEmailAsync(string registrationEmail)
        {
            return await db.Users.SingleOrDefaultAsync(
                r => r.RegisteredEmail == registrationEmail);
        }

        /// <inheritdoc/>
        public async Task<AuthNUser?> FindByUsernameAsync(string username)
        {
            return await db.Users.SingleOrDefaultAsync(
                r => r.Username == username);
        }
    }
}
