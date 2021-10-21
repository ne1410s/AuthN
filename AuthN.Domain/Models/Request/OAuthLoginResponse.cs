namespace AuthN.Domain.Models.Request
{
    /// <summary>
    /// Response following an OAuth login attempt.
    /// </summary>
    public class OAuthLoginResponse
    {
        /// <summary>
        /// Gets login data (if user exists).
        /// </summary>
        public LoginSuccess? Login { get; init; }

        /// <summary>
        /// Gets a prepopulated registration request (if user doesnt exist).
        /// </summary>
        public OAuthRegistrationRequest? Registration { get; init; }
    }
}
