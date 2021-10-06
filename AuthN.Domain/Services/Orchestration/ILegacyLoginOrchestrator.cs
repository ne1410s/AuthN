using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;

namespace AuthN.Domain.Services.Orchestration
{
    /// <summary>
    /// The legacy login orchestrator.
    /// </summary>
    public interface ILegacyLoginOrchestrator
    {
        /// <summary>
        /// Authenticates a user based on a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="OrchestrationException"></exception>
        public Task<LoginSuccess> LegacyLoginAsync(LegacyLoginRequest request);
    }
}
