using System;
using System.ComponentModel.DataAnnotations;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A request to activate following registration in the traditional fashion.
    /// This is typically done from a hyperlink which was emailed to the user.
    /// </summary>
    public class LegacyActivationRequest
    {
        /// <summary>
        /// Gets the email address.
        /// </summary>
        [Required]
        public string Email { get; init; } = default!;

        /// <summary>
        /// Gets the username.
        /// </summary>
        [Required]
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the activation code.
        /// </summary>
        [Required]
        public Guid ActivationCode { get; init; }
    }
}
