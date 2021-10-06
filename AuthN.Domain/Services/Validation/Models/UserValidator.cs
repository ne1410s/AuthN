using AuthN.Domain.Models.Storage;
using FluentValidation;

namespace AuthN.Domain.Services.Validation.Models
{
    /// <summary>
    /// Validates a <see cref="AuthNUser"/> instance.
    /// </summary>
    public class UserValidator : FluentValidatorBase<AuthNUser>
    {
        private readonly CommonRules commonRules;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserValidator"/> class.
        /// </summary>
        /// <param name="commonRules">A common set of rules.</param>
        public UserValidator(CommonRules commonRules)
        {
            this.commonRules = commonRules;
        }

        /// <inheritdoc/>
        protected override void DefineModelValidity()
        {
            var userMinLength = commonRules.UsernameMinLength;
            RuleFor(x => x.Username).NotEmpty().Length(userMinLength, 50);
            RuleFor(x => x.RegistrationEmail).NotEmpty().Length(6, 512);
            RuleFor(x => x.PasswordSalt).NotEmpty().Length(32, 512);
            RuleFor(x => x.PasswordHash).NotEmpty().Length(32, 512);
            RuleFor(x => x.Forename).NotEmpty().Length(2, 50);
            RuleFor(x => x.Surname).NotEmpty().Length(2, 50);
        }
    }
}
