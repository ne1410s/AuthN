using System;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A request to activate following registration in the traditional fashion.
    /// This is typically done from a hyperlink which was emailed to the user.
    /// </summary>
    public class ActivationRequest
    {
        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string EmailAddress { get; init; } = default!;

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the activation code.
        /// </summary>
        public Guid ActivationCode { get; init; }
    }
}
