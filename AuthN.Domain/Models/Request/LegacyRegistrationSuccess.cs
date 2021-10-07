using System;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A result from a successful registration.
    /// </summary>
    public class LegacyRegistrationSuccess
    {
        /// <summary>
        /// Gets the activation code.
        /// </summary>
        public Guid ActivationCode { get; init; }

        /// <summary>
        /// Gets the activation window expiry date and time.
        /// </summary>
        public DateTime ExpiresOn { get; init; }
    }
}
