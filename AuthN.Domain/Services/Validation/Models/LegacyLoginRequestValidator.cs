﻿using AuthN.Domain.Models.Request;
using FluentValidation;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyLoginRequest"/> instance.
    /// </summary>
    public class LegacyLoginRequestValidator
        : FluentValidatorBase<LegacyLoginRequest>
    {
        private readonly CommonRules commonRules;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyLoginRequestValidator"/> class.
        /// </summary>
        /// <param name="commonRules">A common set of rules.</param>
        public LegacyLoginRequestValidator(CommonRules commonRules)
        {
            this.commonRules = commonRules;
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            var userMinLength = commonRules.UsernameMinLength;
            RuleFor(x => x.Username).Length(userMinLength, 50);
            RuleFor(x => x.Username).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Either username or email must be provided.");

            RuleFor(x => x.Email).Length(6, 512);
            RuleFor(x => x.Email).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Username))
                .WithMessage("Either username or email must be provided.");

            var passMinLength = commonRules.PasswordMinLength;
            const string containsText = "'Password' must contain at least one";
            RuleFor(x => x.Password).NotEmpty().Length(passMinLength, 512);
            RuleFor(x => x.Password).Matches("[A-Z]")
                .WithMessage($"{containsText} capital letter");
            RuleFor(x => x.Password).Matches("[a-z]")
                .WithMessage($"{containsText} lower case letter");
            RuleFor(x => x.Password).Matches("\\d")
                .WithMessage($"{containsText} number");
            RuleFor(x => x.Password).Matches("\\W")
                .WithMessage($"{containsText} special character");

            var tokenMaxSeconds = commonRules.TokenMaxMinutes * 60;
            RuleFor(x => x.Duration).InclusiveBetween(5, tokenMaxSeconds);
        }
    }
}