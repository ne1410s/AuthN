using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;

namespace AuthN.Domain.Services.Orchestration.LegacyWorkflow
{
    /// <summary>
    /// The legacy registration orchestrator.
    /// </summary>
    public interface ILegacyRegistrationOrchestrator
    {
        /// <summary>
        /// Registers a user using a traditional workflow; whereby an email will
        /// be sent out for subsequent activation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="OrchestrationException"/>
        public Task<LegacyRegistrationSuccess> LegacyRegisterAsync(
            LegacyRegistrationRequest request);
    }
}
