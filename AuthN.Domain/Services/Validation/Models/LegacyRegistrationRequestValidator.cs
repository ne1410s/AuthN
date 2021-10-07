using AuthN.Domain.Models.Request;
using FluentValidation;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyRegistrationRequest"/> instance.
    /// </summary>
    public class LegacyRegistrationRequestValidator
        : FluentValidatorBase<LegacyRegistrationRequest>
    {
        private readonly CommonRules commonRules;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyRegistrationRequestValidator"/> class.
        /// </summary>
        /// <param name="commonRules">A common set of rules.</param>
        public LegacyRegistrationRequestValidator(CommonRules commonRules)
        {
            this.commonRules = commonRules;
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            var userMinLength = commonRules.UsernameMinLength;
            RuleFor(x => x.Username).NotEmpty().Length(userMinLength, 50);

            var emailMinLength = commonRules.EmailMinLength;
            RuleFor(x => x.Email).EmailAddress()
                .NotEmpty().Length(emailMinLength, 512);

            var passMinLength = commonRules.PasswordMinLength;
            RuleFor(x => x.Password).NotEmpty().Length(passMinLength, 512);
            RuleFor(x => x.Password).IsSufficientlyComplex();

            RuleFor(x => x.Forename).NotEmpty().Length(2, 50);
            RuleFor(x => x.Surname).NotEmpty().Length(2, 50);
        }
    }
}
