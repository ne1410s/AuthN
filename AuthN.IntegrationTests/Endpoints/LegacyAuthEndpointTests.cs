using System;
using System.Net.Http;
using System.Threading.Tasks;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using FluentAssertions;
using Xunit;

namespace AuthN.IntegrationTests.Endpoints
{
    public class LegacyAuthEndpointTests
    {
        private readonly HttpClient client;
        private static readonly AuthNUser activatableUser = new()
        {
            ActivationCode = Guid.NewGuid(),
            ActivationCodeGeneratedOn = DateTime.UtcNow.AddMinutes(-5),
            CreatedOn = DateTime.UtcNow.AddMinutes(-5),
            Forename = "alice",
            Surname = "jones",
            RegisteredEmail = "alice@test.co",
            Username = "alicejones",
        };
        private static readonly AuthNUser activatedUser = new()
        {
            ActivationCode = Guid.NewGuid(),
            ActivationCodeGeneratedOn = DateTime.UtcNow.AddMinutes(-5),
            CreatedOn = DateTime.UtcNow.AddMinutes(-5),
            Forename = "alice",
            Surname = "jones",
            RegisteredEmail = "alice@test.co",
            Username = "alicejones",
        };

        public LegacyAuthEndpointTests()
        {
            var fiveMinutesAgoUtc = DateTime.UtcNow.AddMinutes(-5);
            var appFactory = new IntegrationTestingWebAppFactory(db =>
            {
                db.Users.AddRange(
                    new AuthNUser
                    {

                    });
            });
            client = appFactory.CreateClient();
        }

        [Fact]
        public async Task LegacyWorkflow_RegisterValid_ReturnsSuccess()
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
            var res = await response.ReadJsonAsync<LegacyRegistrationSuccess>();
            res.SuccessData.Should().NotBeNull();
            res.SuccessData!.ActivationCode.Should().NotBeEmpty();
            res.SuccessData!.ExpiresOn.Should().BeAfter(DateTime.UtcNow);
        }

        // Moar registration!




        [Fact]
        public async Task LegacyWorkflow_ActivationValid_ReturnsSuccess()
        {
            // Arrange
            var request = new LegacyActivationRequest
            {
                ActivationCode = activationCode,
                Email = ""
            };

            // Act
            var response = await client.SendJsonAsync(
                "l-auth/register",
                HttpMethod.Post,
                request);

            // Assert
            var res = await response.ReadJsonAsync<LegacyRegistrationSuccess>();
            res.SuccessData.Should().NotBeNull();
            res.SuccessData!.ActivationCode.Should().NotBeEmpty();
            res.SuccessData!.ExpiresOn.Should().BeAfter(DateTime.UtcNow);
        }
    }
}
