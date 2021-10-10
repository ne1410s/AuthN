using System.Collections.Generic;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration;
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
        public void Test1()
        { }

        private static LegacyActivationOrchestrator GetSutWithConfig(
            double windowHours = 24,
            IItemValidator<LegacyActivationRequest>? validator = null,
            IUserRepository? userRepository = null)
        {
            var config = new Dictionary<string, string>
            {
                { "LegacyAuth:ActivationWindowHours", $"{windowHours}" },
            };

            var stubConfig = config.Stub();
            validator ??= Mock.Create<
                IItemValidator<LegacyActivationRequest>>();
            userRepository ??= Mock.Create<IUserRepository>();

            return new LegacyActivationOrchestrator(
                stubConfig,
                validator,
                userRepository);
        }
    }
}
