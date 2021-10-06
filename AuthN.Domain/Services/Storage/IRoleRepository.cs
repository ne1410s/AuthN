using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
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
        public Task<IList<AuthNRole>> ListAsync();

        /// <summary>
        /// Lists all roles assigned to a particular user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A list of user roles.</returns>
        /// <exception cref="DataStateException">User not found.</exception>
        public Task<IList<AuthNRole>> ListForUserAsync(string username);
    }
}
