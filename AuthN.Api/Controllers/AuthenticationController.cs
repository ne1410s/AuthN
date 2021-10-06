using AuthN.Domain.Services.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILegacyLoginOrchestrator authenticator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AuthenticationController"/> class.
        /// </summary>
        public AuthenticationController(
            ILegacyLoginOrchestrator authenticator)
        {
            this.authenticator = authenticator;
        }
    }
}
