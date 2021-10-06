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
    }
}
