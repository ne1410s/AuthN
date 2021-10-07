using System.Collections.Generic;

namespace AuthN.Domain.Models.Storage
{
    /// <summary>
    /// A role.
    /// </summary>
    public record AuthNRole
    {
        /// <summary>
        /// Gets the role name.
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// Gets the users of this role.
        /// </summary>
        public IReadOnlyList<AuthNUser> Users { get; init; } = default!;
    }
}
