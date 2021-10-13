using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Telerik.JustMock;
using Xunit;

namespace AuthN.IntegrationTests.Endpoints
{
    [Collection("Sequential")]
    public class ExceptionHandlerTests
    {
        public enum ErrorBehaviour
        {
            None,
            Invalid,
            DataState,
            Orchestration,
            Other,
        }

        [Theory]
        [InlineData(ErrorBehaviour.None, false, 200)]
        [InlineData(ErrorBehaviour.None, true, 422)]
        [InlineData(ErrorBehaviour.Invalid, false, 422)]
        [InlineData(ErrorBehaviour.DataState, false, 400)]
        [InlineData(ErrorBehaviour.Orchestration, false, 400)]
        [InlineData(ErrorBehaviour.Other, false, 500)]
        public async Task LegacyLogin_ExceptionType_GetsMatchingStatusCode(
            ErrorBehaviour errorBehaviour,
            bool missingRequiredFields,
            int expectedCode)
        {
            // Arrange
            var exception = MakeError(errorBehaviour);
            using var sut = GetFactoryWithBadLogins(exception);
            var client = sut.CreateClient();
            var request = new LegacyLoginRequest
            {
                Username = missingRequiredFields ? string.Empty : "a",
                Password = missingRequiredFields ? string.Empty : "a",
            };

            // Act
            var response = await client.SendJsonAsync(
                "l-auth/login",
                HttpMethod.Post,
                request);

            // Assert
            var result = await response.ReadJsonAsync<LoginSuccess>();
            result.Status.Should().Be((HttpStatusCode)expectedCode);
        }

        /// <summary>
        /// Spins up a demo app where the login endpoint is stubbed to throw
        /// the supplied exception (else an empty result, if null).
        /// </summary>
        /// <param name="exception">The exception to throw on login.</param>
        /// <returns>The web app factory.</returns>
        private static IntegrationTestingWebAppFactory GetFactoryWithBadLogins(
            Exception? exception)
        {
            var stubAuthenticator = Mock.Create<ILegacyLoginOrchestrator>();

            if (exception != null)
            {
                Mock.Arrange(() => stubAuthenticator.LegacyLoginAsync(
                        Arg.IsAny<LegacyLoginRequest>()))
                    .Throws(exception);
            }
            else
            {
                Mock.Arrange(() => stubAuthenticator.LegacyLoginAsync(
                        Arg.IsAny<LegacyLoginRequest>()))
                    .Returns(Task.FromResult(new LoginSuccess()));
            }

            return new IntegrationTestingWebAppFactory(servicesAction: services
                => services.AddTransient(_ => stubAuthenticator));
        }

        /// <summary>
        /// Creates an error indicative of a behaviour type.
        /// </summary>
        /// <param name="type">The error behaviour type.</param>
        /// <returns>An exception.</returns>
        private static Exception? MakeError(ErrorBehaviour type) => type switch
        {
            ErrorBehaviour.None => null,
            ErrorBehaviour.Invalid => new ValidatorException(),
            ErrorBehaviour.DataState => new DataStateException(null),
            ErrorBehaviour.Orchestration => new OrchestrationException(null),
            _ => new Exception(),
        };
    }
}
