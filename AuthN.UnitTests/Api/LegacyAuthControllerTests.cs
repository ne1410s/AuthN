using System.Threading.Tasks;
using AuthN.Api.Controllers;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using Telerik.JustMock;
using Xunit;

namespace AuthN.UnitTests.Api
{
    /// <summary>
    /// Tests for the <see cref="LegacyAuthController"/>.
    /// </summary>
    public class LegacyAuthControllerTests
    {
        [Fact]
        public async Task RegisterAsync_WithRequest_CallsOrchestrator()
        {
            // Arrange
            var mockRegistrar = Mock.Create<ILegacyRegistrationOrchestrator>();
            var sut = GetSut(registrar: mockRegistrar);
            var request = new LegacyRegistrationRequest();

            // Act
            var results = await sut.RegisterAsync(request);

            // Assert
            Mock.Assert(
                () => mockRegistrar.LegacyRegisterAsync(request),
                Occurs.Once());
        }

        [Fact]
        public async Task ActivateAsync_WithRequest_CallsOrchestrator()
        {
            // Arrange
            var mockActivator = Mock.Create<ILegacyActivationOrchestrator>();
            var sut = GetSut(activator: mockActivator);
            var request = new LegacyActivationRequest();

            // Act
            await sut.ActivateAsync(request);

            // Assert
            Mock.Assert(
                () => mockActivator.LegacyActivateAsync(request),
                Occurs.Once());
        }

        [Fact]
        public async Task LoginAsync_WithRequest_CallsOrchestrator()
        {
            // Arrange
            var mockAuthenticator = Mock.Create<ILegacyLoginOrchestrator>();
            var sut = GetSut(authenticator: mockAuthenticator);
            var request = new LegacyLoginRequest();

            // Act
            await sut.LoginAsync(request);

            // Assert
            Mock.Assert(
                () => mockAuthenticator.LegacyLoginAsync(request),
                Occurs.Once());
        }

        private static LegacyAuthController GetSut(
            ILegacyRegistrationOrchestrator? registrar = null,
            ILegacyActivationOrchestrator? activator = null,
            ILegacyLoginOrchestrator? authenticator = null)
        {
            return new LegacyAuthController(
                registrar ?? Mock.Create<ILegacyRegistrationOrchestrator>(),
                activator ?? Mock.Create<ILegacyActivationOrchestrator>(),
                authenticator ?? Mock.Create<ILegacyLoginOrchestrator>());
        }
    }
}
