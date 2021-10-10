using System.Collections.Generic;
using AuthN.Domain.Models.Request;
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
        public void Test1()
        { }

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
