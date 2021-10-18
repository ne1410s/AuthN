using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuthN.Domain.Models.Request;
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
            "id,email,name,first_name,last_name,picture";
        private static readonly string LoginRedirectPath =
            $"{ControllerRoute}/{LoginRedirectRoute}";

        private readonly string clientId = "261081602596593";
        private readonly string appSecret = "5ae1b62114a5fecc6a248ed1ca0688db";
        private readonly HttpClient client;

        public FacebookAuthController(
            IConfiguration config,
            IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient();
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


            return new LoginSuccess
            {
                Token = default!,
                TokenExpiresOn = default,
                User = default!,
            };
        }

        [Authorize]
        [HttpGet]
        [Route("testlol")]
        public void TestLol()
        {

        }
    }
}
