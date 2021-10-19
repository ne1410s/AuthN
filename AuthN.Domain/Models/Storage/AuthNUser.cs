using System;
using System.Collections.Generic;

namespace AuthN.Domain.Models.Storage
{
    /// <summary>
    /// A user.
    /// </summary>
    public class AuthNUser
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        public string? Username { get; init; }

        /// <summary>
        /// Gets the password salt.
        /// </summary>
        public string? PasswordSalt { get; init; }

        /// <summary>
        /// Gets the password hash.
        /// </summary>
        public string? PasswordHash { get; init; }

        /// <summary>
        /// Gets the forename.
        /// </summary>
        public string Forename { get; init; } = default!;

        /// <summary>
        /// Gets the surname.
        /// </summary>
        public string Surname { get; init; } = default!;

        /// <summary>
        /// Gets the email address with which the user is (or is going to be)
        /// activated.
        /// </summary>
        public string RegisteredEmail { get; init; } = default!;

        /// <summary>
        /// Gets or sets the facebook id, if associated.
        /// </summary>
        public string? FacebookId { get; set; }

        /// <summary>
        /// Gets the date the activation code was last generated.
        /// </summary>
        public DateTime? ActivationCodeGeneratedOn { get; init; }

        /// <summary>
        /// Gets the activation code created on traditional registration.
        /// </summary>
        public Guid? ActivationCode { get; init; }

        /// <summary>
        /// Gets the date created.
        /// </summary>
        public DateTime CreatedOn { get; init; }

        /// <summary>
        /// Gets the date activated, if indeed active. 3rd party authentication
        /// registrations are activated immediately whereas traditional requests
        /// require subsequent email verification.
        /// </summary>
        public DateTime? ActivatedOn { get; set; }

        /// <summary>
        /// Gets the privileges assigned directly to the user.
        /// </summary>
        public IReadOnlyList<AuthNPrivilege> Privileges { get; init; } = null!;
    }
}
