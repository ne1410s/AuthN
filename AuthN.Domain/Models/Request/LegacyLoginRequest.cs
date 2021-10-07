using System.ComponentModel.DataAnnotations;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A request to login traditionally; with username and password.
    /// </summary>
    public record LegacyLoginRequest
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        public string? Username { get; init; }

        /// <summary>
        /// Gets the email.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        [Required]
        public string Password { get; init; } = default!;

        /// <summary>
        /// Gets the requested session duration in seconds.
        /// </summary>
        public int? Duration { get; init; }
    }
}
