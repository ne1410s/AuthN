namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// Outcome of a traditional activation.
    /// </summary>
    public enum ActivationOutcome
    {
        /// <summary>
        /// Status unknown. This is only provided to serve as a default.
        /// </summary>
        Inconclusive,

        /// <summary>
        /// The user account was activated successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The user account was already activated.
        /// </summary>
        AlreadyActivated,

        /// <summary>
        /// The activation code has expired. A new one must be request.
        /// </summary>
        CodeExpired,

        /// <summary>
        /// The requested details did not match any applicable users.
        /// </summary>
        NoMatchFound,
    }
}
