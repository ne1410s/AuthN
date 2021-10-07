using Microsoft.AspNetCore.Mvc;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Controller for trusted 3rd party authentication.
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AuthController"/> class.
        /// </summary>
        public AuthController()
        {
        }
    }
}
