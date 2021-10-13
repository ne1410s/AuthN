using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using AuthN.Domain.Services.Security;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using FluentAssertions;
using Telerik.JustMock;
using Xunit;

namespace AuthN.UnitTests.Domain.Orchestrators
{
    /// <summary>
    /// Tests for <see cref="LegacyLoginOrchestrator"/>
    /// </summary>
    public class LegacyLoginOrchestratorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(60)]
        public async Task LegacyLoginAsync_HappyPath_ReturnsSuccess(
            int? tokenDurationSeconds)
        {
            // Arrange
            const int defaultTokenMins = 5;
            var mockRepo = Mock.Create<IUserRepository>();
            var mockValidator = Mock.Create<
                IItemValidator<LegacyLoginRequest>>();
            var sut = GetSutWithConfig(
                defaultTokenMins: defaultTokenMins,
                userRepository: mockRepo,
                validator: mockValidator);
            const string password = "pass";
            var salt = Guid.NewGuid().ToString();
            var user = new AuthNUser
            {
                Username = "bobsmith",
                PasswordSalt = salt,
                PasswordHash = password.Hash(salt),
                Forename = "bob",
                Surname = "smith",
                RegisteredEmail = "email",
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));
            var request = new LegacyLoginRequest
            {
                Username = user.Username,
                Password = password,
                Duration = tokenDurationSeconds,
            };

            // Act
            var result = await sut.LegacyLoginAsync(request);

            // Assert
            var duration = tokenDurationSeconds ?? (defaultTokenMins * 60);
            var expectExpiry = DateTime.Now.AddSeconds(duration);
            var flex = TimeSpan.FromSeconds(3);
            result.User.Should().NotBeNull();
            result.Token.Should().NotBeEmpty();
            result.TokenExpiresOn.Should().BeCloseTo(expectExpiry, flex);
            Mock.Assert(
                () => mockValidator.AssertValid(request),
                Occurs.Once());
        }

        [Fact]
        public async Task LegacyLoginAsync_NoUsername_TriesEmail()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyLoginRequest
            {
                Username = null,
                Email = "email",
            };
            Mock.Arrange(() => mockRepo.FindByEmailAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(null));

            // Act
            Func<Task> act = () => sut.LegacyLoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("User not found");
            Mock.Assert(
                () => mockRepo.FindByEmailAsync(request.Email),
                Occurs.Once());
        }

        [Fact]
        public async Task LegacyLoginAsync_BadPassword_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var salt = Guid.NewGuid().ToString();
            var user = new AuthNUser
            {
                Username = "bobsmith",
                PasswordSalt = salt,
                PasswordHash = "canttouchthis",
                Forename = "bob",
                Surname = "smith",
                RegisteredEmail = "email",
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));
            var request = new LegacyLoginRequest
            {
                Username = user.Username,
                Password = "pass",
            };

            // Act
            Func<Task> act = () => sut.LegacyLoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<OrchestrationException>()
                .WithMessage("Invalid password");
        }

        private static LegacyLoginOrchestrator GetSutWithConfig(
            IUserRepository userRepository,
            string tokenIssuer = "issuer",
            string tokenSecret = "validlengthsecret",
            double defaultTokenMins = 60,
            IItemValidator<LegacyLoginRequest>? validator = null)
        {
            var config = new Dictionary<string, string>
            {
                { "Tokens:Issuer", tokenIssuer },
                { "Tokens:Secret", tokenSecret },
                { "Tokens:DefTokenMinutes", $"{defaultTokenMins}" },
            };

            var stubConfig = config.Stub();
            validator ??= Mock.Create<IItemValidator<LegacyLoginRequest>>();

            return new LegacyLoginOrchestrator(
                stubConfig,
                validator,
                userRepository);
        }
    }
}
