using AuthN.Domain.Models.Storage;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="AuthNPrivilege"/> instance.
    /// </summary>
    public class PrivilegeValidator : FluentValidatorBase<AuthNPrivilege>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PrivilegeValidator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public PrivilegeValidator(IConfiguration config)
            : base(config)
        { }

        /// <inheritdoc/>
        protected override void DefineModelValidity(IConfiguration config)
        {
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
