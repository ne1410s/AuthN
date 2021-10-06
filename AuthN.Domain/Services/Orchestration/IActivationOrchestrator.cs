using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;

namespace AuthN.Domain.Services.Orchestration
{
    /// <summary>
    /// The activation orchestrator; whereby traditionally-registered users have
    /// submitted an activation code that was (e.g.) emailed to them. This is
    /// not required in 3rd party authentication registration workflow, where
    /// activation is implicitly granted on registration.
    /// </summary>
    public interface IActivationOrchestrator
    {
        /// <summary>
        /// Attempts to activate a user, returning the status.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="OrchestrationException"></exception>
        public Task<ActivationOutcome> TryActivateAsync(
            ActivationRequest request);
    }
}
