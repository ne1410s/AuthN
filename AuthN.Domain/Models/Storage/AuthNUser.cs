﻿using System;
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
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the password salt.
        /// </summary>
        public string PasswordSalt { get; internal set; } = default!;

        /// <summary>
        /// Gets the password hash.
        /// </summary>
        public string PasswordHash { get; internal set; } = default!;

        /// <summary>
        /// Gets the forename.
        /// </summary>
        public string Forename { get; internal set; } = default!;

        /// <summary>
        /// Gets the surname.
        /// </summary>
        public string Surname { get; internal set; } = default!;

        /// <summary>
        /// Gets the email address with which the user is (or is going to be)
        /// activated.
        /// </summary>
        public string RegisteredEmail { get; init; } = default!;

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
        /// Gets the roles to which the user belongs.
        /// </summary>
        public IReadOnlySet<AuthNRole> Roles { get; init; } = default!;
    }
}
