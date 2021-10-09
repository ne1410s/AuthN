using AuthN.Domain.Models.Storage;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="AuthNRole"/> instance.
    /// </summary>
    public class RoleValidator : FluentValidatorBase<AuthNRole>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RoleValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public RoleValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches(CommonRegex.KebabCase)
                .Length(4, 30);
        }
    }
}
