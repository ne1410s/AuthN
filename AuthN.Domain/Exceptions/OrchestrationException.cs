using System;

namespace AuthN.Domain.Exceptions
{
    /// <summary>
    /// Represents errors occuring in general domain processing.
    /// </summary>
    public class OrchestrationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the<see cref="DataStateException"/>
        /// class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OrchestrationException(string? message) : base(message)
        { }
    }
}
