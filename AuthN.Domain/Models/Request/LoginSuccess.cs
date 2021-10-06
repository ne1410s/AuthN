using System;
using AuthN.Domain.Models.Storage;

namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// A result from a successful login.
    /// </summary>
    public class LoginSuccess
    {
        /// <summary>
        /// Gets the Json Web Token.
        /// </summary>
        public string Token { get; init; } = default!;

        /// <summary>
        /// Gets the token expiry time.
        /// </summary>
        public DateTime TokenExpiresOn { get; init; }

        /// <summary>
        /// Gets the authenticated user instance.
        /// </summary>
        public AuthNUser User { get; init; } = default!;
    }
}
