using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the email.
        /// </summary>
        [Required]
        public string Email { get; init; } = default!;

        /// <summary>
        /// Gets the password.
        /// </summary>
        [Required]
        public string Password { get; init; } = default!;

        /// <summary>
        /// Gets the forename.
        /// </summary>
        [Required]
        public string Forename { get; init; } = default!;

        /// <summary>
        /// Gets the surname.
        /// </summary>
        [Required]
        public string Surname { get; init; } = default!;
    }
}
