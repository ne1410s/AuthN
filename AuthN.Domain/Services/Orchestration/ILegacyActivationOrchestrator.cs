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
    public interface ILegacyActivationOrchestrator
    {
        /// <summary>
        /// Activates a user who previously registered with the legacy workflow.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="OrchestrationException"/>
        public Task LegacyActivateAsync(LegacyActivationRequest request);
    }
}
