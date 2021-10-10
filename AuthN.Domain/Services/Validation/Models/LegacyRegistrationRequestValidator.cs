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
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyRegistrationRequestValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LegacyRegistrationRequestValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            var minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            var minEmailLength = int.Parse(cSection["MinEmailLength"]);
            var minPasswordLength = int.Parse(cSection["MinPasswordLength"]);

            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(minUsernameLength, 50);
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty()
                .Length(minEmailLength, 512);
            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(minPasswordLength, 512);
            RuleFor(x => x.Password)
                .IsSufficientlyComplex();
            RuleFor(x => x.Forename)
                .NotEmpty()
                .Length(2, 50);
            RuleFor(x => x.Surname)
                .NotEmpty()
                .Length(2, 50);
        }
    }
}
