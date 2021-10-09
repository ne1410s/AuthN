using AuthN.Domain.Models.Request;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="LegacyLoginRequest"/> instance.
    /// </summary>
    public class LegacyLoginRequestValidator
        : FluentValidatorBase<LegacyLoginRequest>
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyLoginRequestValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LegacyLoginRequestValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            var cSection = config.GetSection("Validation");
            var minUsernameLength = int.Parse(cSection["MinUsernameLength"]);
            var minEmailLength = int.Parse(cSection["MinEmailLength"]);
            var minPasswordLength = int.Parse(cSection["MinPasswordLength"]);
            var maxTokenMinutes = double.Parse(cSection["MaxTokenMinutes"]);

            RuleFor(x => x.Username).Length(minUsernameLength, 50);
            RuleFor(x => x.Username).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Either username or email must be provided.");

            RuleFor(x => x.Email).EmailAddress().Length(minEmailLength, 512);
            RuleFor(x => x.Email).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.Username))
                .WithMessage("Either username or email must be provided.");

            RuleFor(x => x.Password).NotEmpty().Length(minPasswordLength, 512);
            RuleFor(x => x.Password).IsSufficientlyComplex();

            var maxTokenSeconds = (int)(maxTokenMinutes * 60);
            RuleFor(x => x.Duration).InclusiveBetween(5, maxTokenSeconds);
        }
    }
}
