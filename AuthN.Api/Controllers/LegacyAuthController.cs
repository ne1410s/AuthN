using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using Microsoft.AspNetCore.Mvc;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Controller for legacy registration, activation and logging-in.
    /// </summary>
    [ApiController]
    [Route("l-auth")]
    public class LegacyAuthController : ControllerBase
    {
        private readonly ILegacyRegistrationOrchestrator legacyRegistrar;
        private readonly ILegacyActivationOrchestrator legacyActivator;
        private readonly ILegacyLoginOrchestrator legacyAuthenticator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyAuthController"/> class.
        /// </summary>
        /// <param name="legacyRegistrar">A legacy registrar.</param>
        /// <param name="legacyActivator">A legacy activator.</param>
        /// <param name="legacyAuthenticator">A legacy authenticator.</param>
        public LegacyAuthController(
            ILegacyRegistrationOrchestrator legacyRegistrar,
            ILegacyActivationOrchestrator legacyActivator,
            ILegacyLoginOrchestrator legacyAuthenticator)
        {
            this.legacyRegistrar = legacyRegistrar;
            this.legacyActivator = legacyActivator;
            this.legacyAuthenticator = legacyAuthenticator;
        }

        /// <summary>
        /// Registers a new user on the system.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="OrchestrationException"/>
        [HttpPost]
        [Route("register")]
        public async Task<LegacyRegistrationSuccess> RegisterAsync(
            LegacyRegistrationRequest request)
        {
            return await legacyRegistrar.LegacyRegisterAsync(request);
        }

        /// <summary>
        /// Activates a user who previously registered in the legacy workflow.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="OrchestrationException"/>
        [HttpPut]
        [Route("activate")]
        public async Task ActivateAsync(LegacyActivationRequest request)
        {
            await legacyActivator.LegacyActivateAsync(request);
        }

        /// <summary>
        /// Obtains a user login using the legacy authentication workflow.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Login security details.</returns>
        /// <exception cref="OrchestrationException"/>
        [HttpPost]
        [Route("login")]
        public async Task<LoginSuccess> LoginAsync(LegacyLoginRequest request)
        {
            return await legacyAuthenticator.LegacyLoginAsync(request);
        }
    }
}
