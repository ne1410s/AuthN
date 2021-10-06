using AuthN.Domain.Services.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Registration controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILegacyRegistrationOrchestrator tradRegistrar;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="RegistrationController"/> class.
        /// </summary>
        /// <param name="tradRegistrar">A traditional registration orchestrator.
        /// </param>
        public RegistrationController(
            ILegacyRegistrationOrchestrator tradRegistrar)
        {
            this.tradRegistrar = tradRegistrar;
        }
    }
}
