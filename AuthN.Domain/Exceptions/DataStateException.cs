namespace AuthN.Domain.Exceptions
{
    /// <summary>
    /// Represents errors occuring due to the state of data in the store.
    /// </summary>
    public class DataStateException : OrchestrationException
    {
        /// <summary>
        /// Initialises a new instance of the<see cref="DataStateException"/>
        /// class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataStateException(string? message) : base(message)
        { }
    }
}
