using AuthN.Domain.Exceptions;

namespace AuthN.Domain.Services.Validation
{
    /// <summary>
    /// Validates an item.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IItemValidator<TItem>
    {
        /// <summary>
        /// Validates an item.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <exception cref="ValidatorException"/>
        public void AssertValid(TItem item);
    }
}
