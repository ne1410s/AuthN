namespace AuthN.Domain.Models.Storage
{
    /// <summary>
    /// Privilege types supported by the system.
    /// </summary>
    public enum PrivilegeType
    {
        /// <summary>
        /// Does not provide any additional permissions.
        /// </summary>
        Default,

        /// <summary>
        /// Provides the permission to assign privileges.
        /// </summary>
        AssignPrivileges,

        /// <summary>
        /// Provides the permission to delete a user.
        /// </summary>
        DeleteUser,
    }
}
