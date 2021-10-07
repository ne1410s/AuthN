using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;

namespace AuthN.Domain.Services.Storage
{
    /// <summary>
    /// Storage repository for the <see cref="AuthNRole"/> model.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Lists all roles in the system.
        /// </summary>
        /// <returns>A list of all roles.</returns>
        public Task<IList<AuthNRole>> ListAllAsync();
    }
}
