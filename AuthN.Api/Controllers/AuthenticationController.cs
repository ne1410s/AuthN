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
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AuthenticationController"/> class.
        /// </summary>
        public AuthenticationController()
        {
        }
    }
}
