using System;
using AuthN.Domain.Models.Request;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyActivationRequest"/> instance.
    /// </summary>
    public class LegacyActivationRequestValidator
        : FluentValidatorBase<LegacyActivationRequest>
    {
        private readonly int minUsernameLength;
        private readonly int minEmailLength;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyLoginRequestValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LegacyActivationRequestValidator(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            minEmailLength = int.Parse(cSection["MinEmailLength"]);
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            RuleFor(x => x.Username).NotEmpty().Length(minUsernameLength, 50);
            RuleFor(x => x.Email).EmailAddress().NotEmpty()
                .Length(minEmailLength, 512);
            RuleFor(x => x.ActivationCode)
                .NotEqual(Guid.Empty);
        }
    }
}
