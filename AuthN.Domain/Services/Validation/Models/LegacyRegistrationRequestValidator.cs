using AuthN.Domain.Models.Request;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyRegistrationRequest"/> instance.
    /// </summary>
    public class LegacyRegistrationRequestValidator
        : FluentValidatorBase<LegacyRegistrationRequest>
    {
        private readonly int minUsernameLength;
        private readonly int minEmailLength;
        private readonly int minPasswordLength;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyRegistrationRequestValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LegacyRegistrationRequestValidator(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            minEmailLength = int.Parse(cSection["MinEmailLength"]);
            minPasswordLength = int.Parse(cSection["MinPasswordLength"]);
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            RuleFor(x => x.Username).NotEmpty().Length(minUsernameLength, 50);
            RuleFor(x => x.Email).EmailAddress().NotEmpty()
                .Length(minEmailLength, 512);
            RuleFor(x => x.Password).NotEmpty().Length(minPasswordLength, 512);
            RuleFor(x => x.Password).IsSufficientlyComplex();
            RuleFor(x => x.Forename).NotEmpty().Length(2, 50);
            RuleFor(x => x.Surname).NotEmpty().Length(2, 50);
        }
    }
}
