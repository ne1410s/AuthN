using System;
using AuthN.Domain.Models.Storage;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="AuthNUser"/> instance.
    /// </summary>
    public class UserValidator : FluentValidatorBase<AuthNUser>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UserValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public UserValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            var minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            var minEmailLength = int.Parse(cSection["MinEmailLength"]);

            RuleFor(x => x.Username).NotEmpty().Length(minUsernameLength, 50);
            RuleFor(x => x.RegisteredEmail).EmailAddress()
                .NotEmpty().Length(minEmailLength, 512);

            RuleFor(x => x.PasswordSalt).NotEmpty().Length(32, 512);
            RuleFor(x => x.PasswordHash).NotEmpty().Length(32, 512);
            RuleFor(x => x.Forename).NotEmpty().Length(2, 50);
            RuleFor(x => x.Surname).NotEmpty().Length(2, 50);
            RuleFor(x => x.CreatedOn).NotEqual(default(DateTime));
        }
    }
}
