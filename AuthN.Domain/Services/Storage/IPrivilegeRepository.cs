using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;

namespace AuthN.Domain.Services.Storage
{
    /// <summary>
    /// Storage repository for the <see cref="AuthNPrivilege"/> model.
    /// </summary>
    public interface IPrivilegeRepository
    {
        /// <summary>
        /// Lists all privileges in the system alphabetically.
        /// </summary>
        /// <returns>A list of all privileges.</returns>
        public Task<IList<AuthNPrivilege>> ListAllAsync();
    }
}
