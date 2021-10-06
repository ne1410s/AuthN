using System;
using System.Security.Cryptography;
using System.Text;
using AuthN.Domain.Models.Storage;

namespace AuthN.Domain.Services.Security
{
    /// <summary>
    /// Extensions providing implementation for security-related tasks.
    /// For common, authoritative, non-interfacing and inexpensive operations.
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Hashes a payload using a salt.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>A base 64 string.</returns>
        public static string Hash(this string payload, string? salt = null)
        {
            var hashable = Encoding.UTF8.GetBytes(payload + salt);
            var hash = SHA256.HashData(hashable);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Creates a Json Web Token from a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="duration">The duration, in seconds.</param>
        /// <returns>A Json Web Token.</returns>
        public static string CreateJwt(this AuthNUser user, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
