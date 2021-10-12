using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuthN.Api.WebModels;

namespace AuthN.IntegrationTests
{
    /// <summary>
    /// Utility methods for working with http.
    /// </summary>
    public static class HttpUtils
    {
        private const string JsonType = "application/json";

        /// <summary>
        /// Sends the requested payload over the requested verb. Designed for
        /// POST, PATCH, PUT where operations typcically require body.
        /// </summary>
        /// <typeparam name="T">The request body object type.</typeparam>
        /// <param name="client">The http client.</param>
        /// <param name="httpMethod">The http method.</param>
        /// <param name="requestObject">The request body object.</param>
        /// <returns>A http response message.</returns>
        public static async Task<HttpResponseMessage> SendJsonAsync<T>(
            this HttpClient client,
            string url,
            HttpMethod httpMethod,
            T requestObject)
        {
            var jsonOpts = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var requestJson = JsonSerializer.Serialize(requestObject, jsonOpts);
            var encoding = Encoding.UTF8;
            var requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestJson, encoding, JsonType),
                Method = httpMethod,
                RequestUri = new Uri(url, UriKind.RelativeOrAbsolute),
            };

            return await client.SendAsync(requestMessage);
        }

        /// <summary>
        /// Reads response json into an instance of the supplied type. If the
        /// response does not indicate success, then the returning type will be
        /// <see cref="HttpErrorResponse"/>.
        /// </summary>
        /// <typeparam name="T">The (success) response type.</typeparam>
        /// <param name="httpResponse">The http response.</param>
        /// <returns>An instance of the specified type.</returns>
        public static async Task<HttpResult<T>> ReadJsonAsync<T>(
            this HttpResponseMessage httpResponse)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            if (httpResponse.IsSuccessStatusCode)
            {
                var deserialised = !string.IsNullOrWhiteSpace(json)
                    ? JsonSerializer.Deserialize<T>(json, opts)
                    : default;

                return new(
                    httpResponse.StatusCode,
                    SuccessData: deserialised,
                    ErrorData: null);
            }
            else
            {
                return new(
                    httpResponse.StatusCode,
                    SuccessData: default,
                    JsonSerializer.Deserialize<HttpErrorResponse>(json, opts));
            }
        }
    }

    public record HttpResult<TSuccess>(
        HttpStatusCode Status,
        TSuccess? SuccessData,
        HttpErrorResponse? ErrorData);
}
