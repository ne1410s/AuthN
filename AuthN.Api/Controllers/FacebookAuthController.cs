using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Security;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AuthN.Api.Controllers
{
    /// <summary>
    /// Controller for facebook authentication.
    /// </summary>
    [Route(ControllerRoute)]
    [ApiController]
    public class FacebookAuthController : ControllerBase
    {
        private const string LoginUrl =
            "https://www.facebook.com/v12.0/dialog/oauth";
        private const string TokenExchangeUrl =
            "https://graph.facebook.com/v12.0/oauth/access_token";
        private const string InfoUrl = "https://graph.facebook.com/me";

        private const string ResponseType = "code";
        private const string ControllerRoute = "fb-auth";
        private const string LoginRedirectRoute = "redirect";
        private const string FieldList =
            "id,email,first_name,last_name";
        private static readonly string LoginRedirectPath =
            $"{ControllerRoute}/{LoginRedirectRoute}";

        private readonly string jwtIssuer;
        private readonly string jwtSecret;
        private readonly uint defaultTokenSecs;
        private readonly string clientId = "261081602596593";
        private readonly string appSecret = "???";
        private readonly HttpClient client;
        private readonly IUserRepository userRepository;
        private readonly IItemValidator<AuthNUser> userValidator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="FacebookAuthController"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="httpClientFactory">The http client factory.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="userValidator">The user validator.</param>
        public FacebookAuthController(
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IUserRepository userRepository,
            IItemValidator<AuthNUser> userValidator)
        {
            jwtIssuer = config["Tokens:Issuer"];
            jwtSecret = config["Tokens:Secret"];
            var defaultTokenMins = config["Tokens:DefTokenMinutes"];
            defaultTokenSecs = (uint)(double.Parse(defaultTokenMins) * 60);

            client = httpClientFactory.CreateClient();
            this.userRepository = userRepository;
            this.userValidator = userValidator;
        }

        /// <summary>
        /// Navigate to this url in the browser.
        /// </summary>
        /// <param name="redirect">The url to redirect to after login.</param>
        [HttpGet]
        [Route("login")]
        public void Login(string redirect)
        {
            var redirHost = Request.Host.Value;
            var redir = $"{Request.Scheme}://{redirHost}/{LoginRedirectPath}";
            
            var referrer = Request.GetTypedHeaders().Referer?.AbsoluteUri;
            var callback = $"{referrer ?? "https://localhost"}/{redirect}";

            var urlBuilder = new StringBuilder(LoginUrl);
            urlBuilder.Append("?client_id=").Append(clientId);
            urlBuilder.Append("&redirect_uri=").Append(redir);
            urlBuilder.Append("&state=").Append(callback);
            urlBuilder.Append("&responseType=").Append(ResponseType);

            Response.Redirect(urlBuilder.ToString());
        }

        /// <summary>
        /// This gets invoked by the provider and relays command back to the ui.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="code">The provider code.</param>
        [HttpGet]
        [Route(LoginRedirectRoute)]
        public void LoginRedirect(string state, string code)
        {
            var urlBuilder = new StringBuilder(state);
            urlBuilder.Append("?code=").Append(code);

            Response.Redirect(urlBuilder.ToString());
        }

        /// <summary>
        /// Gets a token from the interim code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A login success.</returns>
        [HttpGet]
        [Route("token")]
        public async Task<LoginSuccess> GetToken(string code)
        {
            var redirHost = Request.Host.Value;
            var redir = $"{Request.Scheme}://{redirHost}/{LoginRedirectPath}";

            var urlBuilder = new StringBuilder(TokenExchangeUrl);
            urlBuilder.Append("?client_id=").Append(clientId);
            urlBuilder.Append("&redirect_uri=").Append(redir);
            urlBuilder.Append("&client_secret=").Append(appSecret);
            urlBuilder.Append("&code=").Append(code);

            var response = await client.GetAsync(urlBuilder.ToString());
            var body = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            var token = json.GetProperty("access_token").GetString();

            var infoUrlBuilder = new StringBuilder(InfoUrl);
            infoUrlBuilder.Append("?fields=").Append(FieldList);
            infoUrlBuilder.Append("&access_token=").Append(token);
            
            var infoResponse = await client.GetAsync(infoUrlBuilder.ToString());
            var infoBody = await infoResponse.Content.ReadAsStringAsync();
            infoResponse.EnsureSuccessStatusCode();
            var infoJson = JsonSerializer.Deserialize<JsonElement>(infoBody);
            var authEmail = infoJson.GetProperty("email").GetString()
                ?? throw new OrchestrationException("No email received");
            var authId = infoJson.GetProperty("id").GetString()
                ?? throw new OrchestrationException("No id received");

            var repoUser = await userRepository.FindByEmailAsync(authEmail);

            // If no user exists
            if (repoUser == null)
            {
                var authName = infoJson.GetProperty("first_name").GetString();
                var authSurname = infoJson.GetProperty("last_name").GetString();

                repoUser = new AuthNUser
                {
                    CreatedOn = DateTime.UtcNow,
                    FacebookId = authId,
                    Forename = authName!,
                    Surname = authSurname!,
                    RegisteredEmail = authEmail,
                };

                try
                {
                    userValidator.AssertValid(repoUser);
                    await userRepository.AddAsync(repoUser);
                }
                catch (ValidatorException valEx)
                {
                    // Return a 202 or similar, for partially-populated form
                    // Returns user details for later form submission
                }
            }
            else if (authId != repoUser.FacebookId)
            {
                await userRepository.SetFacebookIdAsync(repoUser, authId);
            }

            return repoUser.Tokenise(defaultTokenSecs, jwtIssuer, jwtSecret);
        }

        [Authorize]
        [HttpGet]
        [Route("testlol")]
        public void TestLol()
        {

        }
    }
}
