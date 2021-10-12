using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration;
using Microsoft.Extensions.DependencyInjection;
using Telerik.JustMock;
using Xunit;

namespace AuthN.IntegrationTests.Endpoints
{
    public class ExceptionHandlerTests
    {
        [Fact]
        public async Task LegacyWorkflow_UnhandledException_500()
        {
            // Arrange
            var request = new LegacyRegistrationRequest
            {
                Email = "bob@test.co",
                Forename = "Bob",
                Surname = "Smith",
                Username = "bobsmith",
                Password = "Test123!",
            };

            // Act
            var response = await client.SendJsonAsync(
                "l-auth/register",
                HttpMethod.Post,
                request);

            // Assert
            var res = await response.ReadJsonAsync<object>();
            res.Status.Should().Be(HttpStatusCode.InternalServerError);
            res.SuccessData.Should().BeNull();
            res.ErrorData.Should().NotBeNull();
        }

        private IntegrationTestingWebAppFactory GetLegacySut<TEx>()
            where TEx : Exception
        {
            var stubRegistrar = Mock.Create<ILegacyRegistrationOrchestrator>();
            var stubActivator = Mock.Create<ILegacyActivationOrchestrator>();
            var stubAuthenticator = Mock.Create<ILegacyLoginOrchestrator>();

            Mock.Arrange(() => stubRegistrar.LegacyRegisterAsync(
                    Arg.IsAny<LegacyRegistrationRequest>()))
                .Throws<TEx>();

            return new IntegrationTestingWebAppFactory(
                servicesAction: services =>
                {
                    services.AddTransient(_ => stubRegistrar);

                });
        }
    }
}
