using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Storage;

namespace AuthN.Domain.Services.Storage
{
    /// <summary>
    /// Storage repository for the <see cref="AuthNUser"/> model.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a user to the system.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <exception cref="DataStateException"/>
        public Task AddAsync(AuthNUser user);

        /// <summary>
        /// Activates a user.
        /// </summary>
        /// <exception cref="DataStateException"/>
        public Task ActivateAsync(string username);

        /// <summary>
        /// Gets a user by their username, or null if none found.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A matching user.</returns>
        public Task<AuthNUser?> FindByUsernameAsync(string username);

        /// <summary>
        /// Gets a user by their registration email, or null if none found.
        /// </summary>
        /// <param name="registrationEmail">The original email.</param>
        /// <returns>A matching user.</returns>
        public Task<AuthNUser?> FindByEmailAsync(string registrationEmail);
    }
}
