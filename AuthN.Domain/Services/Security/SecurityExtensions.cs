using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthN.Domain.Models.Storage;
using Microsoft.IdentityModel.Tokens;

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
        /// <param name="durationSeconds">Token duration, in seconds.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="issuer">The issuing application.</param>
        /// <returns>A Json Web Token.</returns>
        public static string CreateJwt(
            this AuthNUser user,
            int durationSeconds,
            string signingKey,
            string issuer)
        {
            var keyBytes = Encoding.UTF8.GetBytes(signingKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            const string algorithm = SecurityAlgorithms.HmacSha256;
            var credentials = new SigningCredentials(securityKey, algorithm);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.RegisteredEmail),
                new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Forename),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.Surname),
                new Claim("Roles", JsonSerializer.Serialize(user.Roles)),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(durationSeconds),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
