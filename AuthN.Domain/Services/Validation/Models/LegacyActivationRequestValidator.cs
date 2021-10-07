using System;
using AuthN.Domain.Models.Request;
using FluentValidation;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyActivationRequest"/> instance.
    /// </summary>
    public class LegacyActivationRequestValidator
        : FluentValidatorBase<LegacyActivationRequest>
    {
        private readonly CommonRules commonRules;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyLoginRequestValidator"/> class.
        /// </summary>
        /// <param name="commonRules">A common set of rules.</param>
        public LegacyActivationRequestValidator(CommonRules commonRules)
        {
            this.commonRules = commonRules;
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(commonRules.UsernameMinLength, 50);
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty()
                .Length(commonRules.EmailMinLength, 512);
            RuleFor(x => x.ActivationCode)
                .NotEqual(Guid.Empty);
        }
    }
}
