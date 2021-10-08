using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using Microsoft.EntityFrameworkCore;

namespace AuthN.Persistence.Repositories
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
            return await db.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
