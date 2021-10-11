using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Orchestration;
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
        [Fact]
        public async Task LegacyLoginAsync_HappyPath_ReturnsSuccess()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var mockValidator = Mock.Create<
                IItemValidator<LegacyLoginRequest>>();
            var sut = GetSutWithConfig(
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
            };

            // Act
            var result = await sut.LegacyLoginAsync(request);

            // Assert
            result?.User.Should().NotBeNull();
            result!.Token.Should().NotBeEmpty();
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
            string tokenIssuer = "issuer",
            string tokenSecret = "validlengthsecret",
            double defaultTokenMins = 60,
            IItemValidator<LegacyLoginRequest>? validator = null,
            IUserRepository? userRepository = null)
        {
            var config = new Dictionary<string, string>
            {
                { "Tokens:Issuer", tokenIssuer },
                { "Tokens:Secret", tokenSecret },
                { "Tokens:DefTokenMinutes", $"{defaultTokenMins}" },
            };

            var stubConfig = config.Stub();
            validator ??= Mock.Create<IItemValidator<LegacyLoginRequest>>();
            userRepository ??= Mock.Create<IUserRepository>();

            return new LegacyLoginOrchestrator(
                stubConfig,
                validator,
                userRepository);
        }
    }
}
