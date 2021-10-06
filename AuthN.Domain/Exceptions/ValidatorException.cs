using System.Collections.Generic;
using AuthN.Domain.Services.Validation;

namespace AuthN.Domain.Exceptions
{
    /// <summary>
    /// Represents errors occuring during validation.
    /// </summary>
    public class ValidatorException : OrchestrationException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ValidatorException"/>
        /// class.
        /// </summary>
        /// <param name="invalidItems">Invalid items.</param>
        public ValidatorException(params InvalidItem[] invalidItems)
        {
            InvalidItems = invalidItems;
        }

        /// <summary>
        /// Gets the invalid items.
        /// </summary>
        public IList<InvalidItem> InvalidItems { get; init; }
    }
}
