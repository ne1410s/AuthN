using AuthN.Domain.Models.Storage;
using FluentValidation;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="AuthNRole"/> instance.
    /// </summary>
    public class RoleValidator : FluentValidatorBase<AuthNRole>
    {
        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches(CommonRegex.KebabCase)
                .Length(4, 30);
        }
    }
}
