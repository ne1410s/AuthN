using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AuthN.Api.WebModels;
using AuthN.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AuthN.Api.Middleware
{
    /// <summary>
    /// Exception handling middleware.
    /// </summary>
    public class ExceptionHandler
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="next">The request processing.</param>
        public ExceptionHandler(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                HttpStatusCode responseCode;
                HttpErrorResponse response;
                var errorType = ex.GetType().Name;
                if (ex is ValidatorException valEx)
                {
                    var errors = valEx.InvalidItems.Select(e => e.ErrorMessage);
                    responseCode = HttpStatusCode.UnprocessableEntity;
                    response = new(errorType, valEx.Message, errors.ToArray());
                }
                else if (ex is OrchestrationException orchEx)
                {
                    responseCode = HttpStatusCode.BadRequest;
                    response = new(errorType, orchEx.Message);
                }
                else
                {
                    responseCode = HttpStatusCode.InternalServerError;
                    response = new(errorType, ex.Message);
                }

                httpContext.Response.StatusCode = (int)responseCode;
                httpContext.Response.ContentType = "application/json";
                var jsonOpts = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                var responseJson = JsonSerializer.Serialize(response, jsonOpts);
                await httpContext.Response.WriteAsync(responseJson);
            }
        }
    }
}
