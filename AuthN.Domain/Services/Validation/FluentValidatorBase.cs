using System.Linq;
using AuthN.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace AuthN.Domain.Services.Validation
{
    /// <summary>
    /// Validates a model of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class FluentValidatorBase<T> : AbstractValidator<T>,
        IItemValidator<T>
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="FluentValidatorBase{T}"/> class.
        /// </summary>
        protected FluentValidatorBase()
        {
            this.DefineModelValidity();
        }

        /// <inheritdoc/>
        public virtual void AssertValid(T item)
        {
            ValidationResult result = Validate(item);
            if (!result.IsValid)
            {
                throw new ValidatorException
                {
                    InvalidItems = result.Errors.Select(e => new InvalidItem
                    {
                        ErrorMessage = e.ErrorMessage,
                        Property = e.PropertyName,
                        AttemptedValue = e.AttemptedValue,
                    }).ToArray()
                };
            }
        }

        /// <summary>
        /// Defines the fluent validation rules.
        /// </summary>
        protected abstract void DefineModelValidity();
    }
}
