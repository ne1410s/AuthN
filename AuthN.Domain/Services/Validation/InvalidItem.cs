namespace AuthN.Domain.Services.Validation
{
    /// <summary>
    /// Represents an invalid item.
    /// </summary>
    public record InvalidItem
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; init; } = default!;

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Property { get; init; } = default!;

        /// <summary>
        /// Gets the attempted (invalid) value.
        /// </summary>
        public object AttemptedValue { get; init; } = default!;
    }
}
