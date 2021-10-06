using System;
using System.Threading.Tasks;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Storage;

namespace AuthN.Domain.Services.Orchestration
{
    /// <inheritdoc cref="IActivationOrchestrator"/>
    public class ActivationOrchestrator : IActivationOrchestrator
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="ActivationOrchestrator"/> class.
        /// </summary>
        public ActivationOrchestrator(
            IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<ActivationOutcome> TryActivateAsync(
            ActivationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
