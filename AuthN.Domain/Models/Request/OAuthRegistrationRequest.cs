using System;
using System.ComponentModel.DataAnnotations;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// Completes registration for OAuth workflows.
    /// </summary>
    public class OAuthRegistrationRequest
    {
        /// <summary>
        /// Gets the provider id.
        /// </summary>
        [Required]
        public string ProviderId { get; init; } = default!;

        /// <summary>
        /// Gets the provider access code.
        /// </summary>
        [Required]
        public string ProviderCode { get; init; } = default!;

        /// <summary>
        /// Gets the email.
        /// </summary>
        [Required]
        public string Email { get; init; } = default!;

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

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; init; }

        /// <summary>
        /// Gets a checksum.
        /// </summary>
        [Required]
        public string Checksum { get; init; } = default!;
    }
}
