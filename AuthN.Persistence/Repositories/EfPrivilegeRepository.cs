using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using Microsoft.EntityFrameworkCore;

namespace AuthN.Persistence.Repositories
{
    /// <inheritdoc cref="IPrivilegeRepository"/>
    public class EfPrivilegeRepository : IPrivilegeRepository
    {
        private readonly AuthNDbContext db;

        /// <summary>
        /// Initialises a new instance of the <see cref="EfPrivilegeRepository"/>
        /// class.
        /// </summary>
        public EfPrivilegeRepository(AuthNDbContext db)
        {
            this.db = db;
        }

        /// <inheritdoc/>
        public async Task<IList<AuthNPrivilege>> ListAllAsync()
        {
            await Task.CompletedTask;
            return db.Privileges
                .AsEnumerable()
                .OrderBy(r => r.Type.ToString())
                .ToList();
        }
    }
}
