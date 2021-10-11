using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Orchestration;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using FluentAssertions;
using Telerik.JustMock;
using Xunit;

namespace AuthN.UnitTests.Domain.Orchestrators.Orchestrators
{
    /// <summary>
    /// Tests for <see cref="LegacyRegistrationOrchestrator"/>
    /// </summary>
    public class LegacyRegistrationOrchestratorTests
    {
        [Fact]
        public async Task LegacyRegisterAsync_HappyPath_ReturnsSuccess()
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var mockValidator = Mock.Create<
                IItemValidator<LegacyRegistrationRequest>>();
            var sut = GetSutWithConfig(
                userRepository: mockRepo,
                validator: mockValidator);
            Mock.Arrange(() => mockRepo.FindByEmailAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(null));
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(null));
            var request = new LegacyRegistrationRequest
            {
                Email = "email",
                Password = "pass",
            };

            // Act
            var result = await sut.LegacyRegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ActivationCode.Should().NotBeEmpty();
            result.ExpiresOn.Should().NotBe(default);
            Mock.Assert(
                () => mockValidator.AssertValid(request),
                Occurs.Once());
            Mock.Assert(
                () => mockRepo.AddAsync(Arg.IsAny<AuthNUser>()),
                Occurs.Once());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LegacyRegisterAsync_EmailExists_ThrowsException(
            bool matchWasActivated)
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var user = new AuthNUser
            {
                ActivatedOn = matchWasActivated ? DateTime.Today : null,
            };
            Mock.Arrange(() => mockRepo.FindByEmailAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));
            var request = new LegacyRegistrationRequest();
            var expectedMessage = matchWasActivated
                ? "This email is taken"
                : "This email is awaiting activation";

            // Act
            Func<Task> act = () => sut.LegacyRegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage(expectedMessage);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LegacyRegisterAsync_UsernameExists_ThrowsException(
            bool matchWasActivated)
        {
            // Arrange
            var mockRepo = Mock.Create<IUserRepository>();
            var sut = GetSutWithConfig(userRepository: mockRepo);
            var user = new AuthNUser
            {
                ActivatedOn = matchWasActivated ? DateTime.Today : null,
            };
            Mock.Arrange(() => mockRepo.FindByEmailAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(null));
            Mock.Arrange(() => mockRepo.FindByUsernameAsync(Arg.AnyString))
                .Returns(Task.FromResult<AuthNUser?>(user));
            var request = new LegacyRegistrationRequest();
            var expectedMessage = matchWasActivated
                ? "This username is taken"
                : "This username is awaiting activation";

            // Act
            Func<Task> act = () => sut.LegacyRegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<DataStateException>()
                .WithMessage(expectedMessage);
        }

        private static LegacyRegistrationOrchestrator GetSutWithConfig(
            double windowHours = 24,
            IItemValidator<LegacyRegistrationRequest>? validator = null,
            IUserRepository? userRepository = null)
        {
            var config = new Dictionary<string, string>
            {
                { "LegacyAuth:ActivationWindowHours", $"{windowHours}" },
            };

            var stubConfig = config.Stub();
            validator ??= Mock.Create<
                IItemValidator<LegacyRegistrationRequest>>();
            userRepository ??= Mock.Create<IUserRepository>();

            return new LegacyRegistrationOrchestrator(
                stubConfig,
                validator,
                userRepository);
        }
    }
}
