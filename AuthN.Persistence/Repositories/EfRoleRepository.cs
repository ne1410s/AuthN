using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using AuthN.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Afi.Registration.Persistence.Repositories
{
    /// <inheritdoc cref="IRoleRepository"/>
    public class EfRoleRepository : IRoleRepository
    {
        private readonly AuthNDbContext db;

        /// <summary>
        /// Initialises a new instance of the <see cref="EfRoleRepository"/>
        /// class.
        /// </summary>
        public EfRoleRepository(AuthNDbContext db)
        {
            this.db = db;
        }

        /// <inheritdoc/>
        public async Task<IList<AuthNRole>> ListAllAsync()
        {
            return await db.Roles.ToListAsync();
        }
    }
}
