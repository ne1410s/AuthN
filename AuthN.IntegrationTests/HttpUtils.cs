using System;
using System.Collections.Generic;
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
        /// <param name="headers">Any request headers.</param>
        /// <returns>A http response message.</returns>
        public static async Task<HttpResponseMessage> SendJsonAsync<T>(
            this HttpClient client,
            string url,
            HttpMethod httpMethod,
            T requestObject,
            IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            var requestJson = JsonSerializer.Serialize(requestObject);
            var encoding = Encoding.UTF8;
            var requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestJson, encoding, JsonType),
                Method = httpMethod,
                RequestUri = new Uri(url, UriKind.RelativeOrAbsolute),
            };

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    requestMessage.Headers.Add(kvp.Key, kvp.Value);
                }
            }

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
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            if (httpResponse.IsSuccessStatusCode)
            {
                return new(
                    httpResponse.StatusCode,
                    JsonSerializer.Deserialize<T>(responseJson, jsonOpts),
                    ErrorData: null);
            }
            else
            {
                return new(
                    httpResponse.StatusCode,
                    SuccessData: default,
                    ErrorData: JsonSerializer.Deserialize<HttpErrorResponse>(
                        responseJson,
                        jsonOpts));
            }
        }
    }

    public record HttpResult<TSuccess>(
        HttpStatusCode Status,
        TSuccess? SuccessData,
        HttpErrorResponse? ErrorData);
}
