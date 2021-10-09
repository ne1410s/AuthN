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
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyLoginRequestValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LegacyActivationRequestValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            var minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            var minEmailLength = int.Parse(cSection["MinEmailLength"]);

            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(minUsernameLength, 50);
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty()
                .Length(minEmailLength, 512);
            RuleFor(x => x.ActivationCode)
                .NotEmpty();
        }
    }
}
