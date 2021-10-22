using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Security;
using FluentAssertions;
using Xunit;

namespace AuthN.IntegrationTests.Endpoints
{
    [Collection("Sequential")]
    public class LegacyAuthEndpointTests
    {
        private const string ActivatedUserPass = "Test123!";
        private const string ActivatedUserSalt = "random";

        private const string ActivateeUserPass = "Test456!";
        private const string ActivateeUserSalt = "other-thing";

        private static readonly AuthNUser ActivateeUser = new()
        {
            ActivationCode = Guid.NewGuid(),
            ActivationCodeGeneratedOn = DateTime.UtcNow.AddMinutes(-5),
            CreatedOn = DateTime.UtcNow.AddMinutes(-5),
            Forename = "alice",
            Surname = "jones",
            RegisteredEmail = "alice@test.co",
            PasswordSalt = ActivateeUserSalt,
            PasswordHash = ActivateeUserPass.Hash(ActivateeUserSalt),
            Username = "alicejones",
        };
        private static readonly AuthNUser ActivatedUser = new()
        {
            ActivationCode = Guid.NewGuid(),
            ActivationCodeGeneratedOn = DateTime.UtcNow.AddMinutes(-5),
            ActivatedOn = DateTime.UtcNow.AddMinutes(-3),
            CreatedOn = DateTime.UtcNow.AddMinutes(-5),
            Forename = "fred",
            Surname = "brown",
            RegisteredEmail = "fred@test.co",
            PasswordSalt = ActivatedUserSalt,
            PasswordHash = ActivatedUserPass.Hash(ActivatedUserSalt),
            Username = "fredbrown",
        };

        private readonly HttpClient client;

        public LegacyAuthEndpointTests()
        {
            var fiveMinutesAgoUtc = DateTime.UtcNow.AddMinutes(-5);
            var appFactory = new IntegrationTestingWebAppFactory(db =>
            {
                db.Users.AddRange(
                    ActivatedUser,
                    ActivateeUser);
            });
            client = appFactory.CreateClient();
        }

        [Fact]
        public async Task LegacyWorkflow_ValidRegistration_ReturnsSuccess()
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

        [Fact]
        public async Task LegacyWorkflow_ValidActivation_ReturnsSuccess()
        {
            // Arrange
            var request = new LegacyActivationRequest
            {
                ActivationCode = ActivateeUser.ActivationCode!.Value,
                Email = ActivateeUser.RegisteredEmail,
                Username = ActivateeUser.Username!,
            };

            // Act
            var response = await client.SendJsonAsync(
                "l-auth/activate",
                HttpMethod.Put,
                request);

            // Assert
            var res = await response.ReadJsonAsync<object>();
            res.Status.Should().Be(HttpStatusCode.OK);
            res.ErrorData.Should().BeNull();
        }

        [Fact]
        public async Task LegacyWorkflow_ValidLogin_ReturnsSuccess()
        {
            // Arrange
            var request = new LegacyLoginRequest
            {
                Username = ActivatedUser.Username,
                Password = ActivatedUserPass,
            };

            // Act
            var response = await client.SendJsonAsync(
                "l-auth/login",
                HttpMethod.Post,
                request);

            // Assert
            var res = await response.ReadJsonAsync<LoginSuccess>();
            res.SuccessData.Should().NotBeNull();
            res.SuccessData!.Token.Should().NotBeNullOrWhiteSpace();
            res.SuccessData!.TokenExpiresOn.Should().BeAfter(DateTime.Now);
            res.SuccessData!.User.Surname.Should().Be(ActivatedUser.Surname);
            res.ErrorData.Should().BeNull();
        }

        [Fact]
        public async Task LegacyWorkflow_E2E()
        {
            // Register
            var registerRequest = new LegacyRegistrationRequest
            {
                Email = "kate@test.co",
                Forename = "Kate",
                Surname = "Taylor",
                Username = "katetaylor",
                Password = "Test789!",
            };
            var registerResponse = await client.SendJsonAsync(
                "l-auth/register",
                HttpMethod.Post,
                registerRequest);
            var registerResult = await registerResponse
                .ReadJsonAsync<LegacyRegistrationSuccess>();

            // Activate
            var activateRequest = new LegacyActivationRequest
            {
                ActivationCode = registerResult.SuccessData!.ActivationCode,
                Email = registerRequest.Email,
                Username = registerRequest.Username,
            };
            await client.SendJsonAsync(
                "l-auth/activate",
                HttpMethod.Put,
                activateRequest);

            // Login
            var loginRequest = new LegacyLoginRequest
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
            };
            var loginResponse = await client.SendJsonAsync(
                "l-auth/login",
                HttpMethod.Post,
                loginRequest);
            var loginResult = await loginResponse.ReadJsonAsync<LoginSuccess>();

            // Assert
            loginResult.SuccessData!.Token.Should().NotBeEmpty();
        }
    }
}
