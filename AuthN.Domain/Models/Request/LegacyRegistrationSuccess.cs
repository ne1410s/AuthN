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
    }
}
