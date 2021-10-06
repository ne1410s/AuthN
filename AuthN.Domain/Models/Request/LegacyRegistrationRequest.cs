namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A request to register in a traditional fashion; the username, password,
    /// personal details are supplied explicity. Activation is later required.
    /// </summary>
    public class LegacyRegistrationRequest
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get; init; } = default!;

        /// <summary>
        /// Gets the forename.
        /// </summary>
        public string Forename { get; init; } = default!;

        /// <summary>
        /// Gets the surname.
        /// </summary>
        public string Surname { get; init; } = default!;

        /// <summary>
        /// Gets the email.
        /// </summary>
        public string Email { get; init; } = default!;
    }
}
