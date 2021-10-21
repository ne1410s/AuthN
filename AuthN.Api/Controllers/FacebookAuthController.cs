using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private const string PermsList = "email,user_birthday";
        private const string FieldList =
            "id,email,first_name,last_name,birthday";
        private static readonly CultureInfo YankeeCulture = new("en-US");
        private static readonly string LoginRedirectPath =
            $"{ControllerRoute}/{LoginRedirectRoute}";

        private readonly string jwtIssuer;
        private readonly string jwtSecret;
        private readonly uint tokenSeconds;
        private readonly string clientId = "261081602596593";
        private readonly string appSecret = "636be92ae449064d8c7cc00470b1a27d";
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
            tokenSeconds = (uint)(double.Parse(defaultTokenMins) * 60);

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
        public void LoginWithFacebook(string redirect)
        {
            var redirHost = Request.Host.Value;
            var redir = $"{Request.Scheme}://{redirHost}/{LoginRedirectPath}";

            var referrer = Request.GetTypedHeaders().Referer?.AbsoluteUri;
            var callback = (referrer ?? "https://localhost/") + redirect;

            var urlBuilder = new StringBuilder(LoginUrl);
            urlBuilder.Append("?client_id=").Append(clientId);
            urlBuilder.Append("&redirect_uri=").Append(redir);
            urlBuilder.Append("&state=").Append(callback);
            urlBuilder.Append("&responseType=").Append(ResponseType);
            urlBuilder.Append("&scope=").Append(PermsList);

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
        /// Logs in internally, based on the previously obtained provider code.
        /// The user may or may not exist already; this is reflected in the
        /// response.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A login response.</returns>
        [HttpGet]
        [Route("login_int")]
        public async Task<OAuthLoginResponse> LoginInternally(string code)
        {
            var data = await GetDataAsync(code);
            var dbUser = await userRepository.FindByEmailAsync(data.Email);

            // If no user exists
            if (dbUser == null)
            {
                return new()
                {
                    Registration = new()
                    {
                        ProviderId = data.Id,
                        Email = data.Email,
                        Forename = data.FirstName,
                        Surname = data.LastName,
                        DateOfBirth = data.DateOfBirth,
                    }
                };
            }

            // If user exists, ensure their provider id is up to date
            if (dbUser.FacebookId != data.Id)
            {
                await userRepository.SetFacebookIdAsync(dbUser, data.Id);
            }

            return new()
            {
                Login = dbUser.Tokenise(tokenSeconds, jwtIssuer, jwtSecret)
            };
        }

        [HttpPost]
        [Route("register")]
        public async Task<LoginSuccess> Register(
            [FromForm]OAuthRegistrationRequest req)
        {
            var data = await GetDataAsync(req.ProviderCode);
            if (data.Id != req.ProviderId || data.Email != req.Email)
            {
                throw new OrchestrationException("Data anomaly");
            }

            var newUser = new AuthNUser
            {
                ActivatedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow,
                DateOfBirth = req.DateOfBirth,
                FacebookId = req.ProviderId,
                Forename = req.Forename,
                Surname = req.Surname,
                RegisteredEmail = req.Email,
            };

            userValidator.AssertValid(newUser);
            await userRepository.AddAsync(newUser);

            return newUser.Tokenise(tokenSeconds, jwtIssuer, jwtSecret);
        }

        [Authorize]
        [HttpGet]
        [Route("testlol")]
        public void TestLol()
        {

        }

        private class FacebookUserData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = default!;
            
            [JsonPropertyName("email")]
            public string Email { get; set; } = default!;

            [JsonPropertyName("first_name")]
            public string FirstName { get; set; } = default!;

            [JsonPropertyName("last_name")]
            public string LastName { get; set; } = default!;

            [JsonPropertyName("birthday")]
            public string Birthday { get; set; } = default!;

            public DateTime DateOfBirth => DateTime.ParseExact(
                Birthday,
                "MM/dd/yyyy",
                YankeeCulture,
                DateTimeStyles.None);
        }

        private async Task<FacebookUserData> GetDataAsync(string code)
        {
            var redirHost = Request.Host.Value;
            var redir = $"{Request.Scheme}://{redirHost}/{LoginRedirectPath}";

            var urlBuilder = new StringBuilder(TokenExchangeUrl);
            urlBuilder.Append("?client_id=").Append(clientId);
            urlBuilder.Append("&redirect_uri=").Append(redir);
            urlBuilder.Append("&client_secret=").Append(appSecret);
            urlBuilder.Append("&code=").Append(code);

            var tokenResponse = await client.GetAsync(urlBuilder.ToString());
            var tokenBody = await tokenResponse.Content.ReadAsStringAsync();
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = JsonSerializer.Deserialize<JsonElement>(tokenBody);
            var extToken = tokenJson.GetProperty("access_token").GetString();

            var infoUrlBuilder = new StringBuilder(InfoUrl);
            infoUrlBuilder.Append("?fields=").Append(FieldList);
            infoUrlBuilder.Append("&access_token=").Append(extToken);

            var response = await client.GetAsync(infoUrlBuilder.ToString());
            var body = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<FacebookUserData>(body)!;
        }
    }
}
