using System.Collections.Generic;

namespace AuthN.Domain.Models.Storage
{
    /// <summary>
    /// A privilege.
    /// </summary>
    public record AuthNPrivilege
    {
        /// <summary>
        /// Gets the privilege type.
        /// </summary>
        public PrivilegeType Type { get; init; }

        /// <summary>
        /// Gets the users with this privilege.
        /// </summary>
        public IReadOnlyList<AuthNUser> Users { get; init; } = default!;
    }
}
