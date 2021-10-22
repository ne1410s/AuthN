using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuthN.Domain.Models.Request;
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
        /// <exception cref="ArgumentException"/>
        public static string CreateJwt(
            this AuthNUser user,
            uint durationSeconds,
            string signingKey,
            string issuer)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentException("Issuer is required");
            }

            if (signingKey == null || signingKey.Length < 16)
            {
                throw new ArgumentException("Key must be >= 16 characters");
            }

            if (durationSeconds == 0)
            {
                throw new ArgumentException("Duration must be > 0 seconds");
            }

            var keyBytes = Encoding.UTF8.GetBytes(signingKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            const string algorithm = SecurityAlgorithms.HmacSha256;
            var credentials = new SigningCredentials(securityKey, algorithm);
            var privs = user.Privileges?.Select(r => r.Type)
                ?? Array.Empty<PrivilegeType>();

            var opts = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            opts.Converters.Add(new JsonStringEnumConverter());

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Sub, $"{user.UserId}"),
                new Claim(JwtRegisteredClaimNames.Email, user.RegisteredEmail),
                new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Forename),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.Surname),
                new Claim("Privileges", JsonSerializer.Serialize(privs, opts)),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(durationSeconds),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates an access token in an object representing login success.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="tokenDuration">The token duration.</param>
        /// <param name="tokenIssuer">The token issuer.</param>
        /// <param name="tokenSecret">The token secret (signing key).</param>
        /// <returns>Login success object.</returns>
        /// <exception cref="ArgumentException"/>
        public static LoginSuccess Tokenise(
            this AuthNUser user,
            uint tokenDuration,
            string tokenIssuer,
            string tokenSecret)
        {
            var expiry = DateTime.Now.AddSeconds(tokenDuration);
            return new LoginSuccess
            {
                User = user,
                Token = user.CreateJwt(tokenDuration, tokenSecret, tokenIssuer),
                TokenExpiresOn = expiry,
            };
        }
    }
}
