using AuthN.Domain.Services.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Activation controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ActivationController : ControllerBase
    {
        private readonly IActivationOrchestrator activator;

        /// <summary>
        /// Initialises a new instance of the <see cref="ActivationController"/>
        /// class.
        /// </summary>
        public ActivationController(
            IActivationOrchestrator activator)
        {
            this.activator = activator;
        }
    }
}
