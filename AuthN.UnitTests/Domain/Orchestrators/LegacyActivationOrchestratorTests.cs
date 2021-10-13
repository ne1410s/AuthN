using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using FluentAssertions;
using Telerik.JustMock;
using Xunit;

namespace AuthN.UnitTests.Domain
{
    /// <summary>
    /// Tests for <see cref="LegacyActivationOrchestrator"/>
    /// </summary>
    public class LegacyActivationOrchestratorTests
    {
        [Fact]
        public async Task LegacyActivateAsync_HappyPath_CallsActivate()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var mockValidator = Mock.Create<
                IItemValidator<LegacyActivationRequest>>();
            var sut = GetSutWithConfig(
                userRepository: mockRepo,
                validator: mockValidator);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = request.ActivationCode,
                ActivationCodeGeneratedOn = DateTime.UtcNow.AddSeconds(-5),
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            await sut.LegacyActivateAsync(request);

            // Assert
            Mock.Assert(
                () => mockValidator.AssertValid(request),
                Occurs.Once());
            Mock.Assert(
                () => mockRepo.ActivateAsync(Arg.AnyString),
                Occurs.Once());
        }

        [Fact]
        public async Task LegacyActivateAsync_NoUserFound_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(null));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("No matching users found.");
        }

        [Fact]
        public async Task LegacyActivateAsync_MismatchCode_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = Guid.NewGuid(),
                ActivationCodeGeneratedOn = DateTime.UtcNow.AddSeconds(-5),
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("No matching users found.");
        }

        [Fact]
        public async Task LegacyActivateAsync_MissingCode_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = null,
                ActivationCodeGeneratedOn = DateTime.UtcNow.AddSeconds(-5),
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("No matching users found.");
        }

        [Fact]
        public async Task LegacyActivateAsync_MissingGenDate_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = request.ActivationCode,
                ActivationCodeGeneratedOn = null,
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("Missing activation timestamp.");
        }

        [Fact]
        public async Task LegacyActivateAsync_MismatchEmail_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = request.ActivationCode,
                ActivationCodeGeneratedOn = DateTime.UtcNow.AddSeconds(-5),
                RegisteredEmail = "different-email",
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage("No matching users found.");
        }

        [Fact]
        public async Task LegacyActivateAsync_AlreadyActivated_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = request.ActivationCode,
                ActivationCodeGeneratedOn = DateTime.UtcNow.AddSeconds(-5),
                ActivatedOn = DateTime.UtcNow.AddSeconds(-1),
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<OrchestrationException>()
                .WithMessage("User is already activated.");
        }

        [Fact]
        public async Task LegacyActivateAsync_ExpiredWindow_ThrowsException()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            const double windowHours = 24;
            var sut = GetSutWithConfig(mockRepo, windowHours);
            var request = new LegacyActivationRequest { Email = "email" };
            var user = new AuthNUser
            {
                ActivationCode = request.ActivationCode,
                ActivationCodeGeneratedOn = DateTime.UtcNow
                    .AddHours(-windowHours)
                    .AddMinutes(-5),
                RegisteredEmail = request.Email,
            };
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));

            // Act
            Func<Task> act = () => sut.LegacyActivateAsync(request);

            // Assert
            await act.Should().ThrowAsync<OrchestrationException>()
                .WithMessage("Activation code expired.");
        }

        private static LegacyActivationOrchestrator GetSutWithConfig(
            IUserRepository userRepository,
            double windowHours = 24,
            IItemValidator<LegacyActivationRequest>? validator = null)
        {
            var config = new Dictionary<string, string>
            {
                { "LegacyAuth:ActivationWindowHours", $"{windowHours}" },
            };

            var stubConfig = config.Stub();
            validator ??= Mock.Create<
                IItemValidator<LegacyActivationRequest>>();

            return new LegacyActivationOrchestrator(
                stubConfig,
                validator,
                userRepository);
        }
    }
}
