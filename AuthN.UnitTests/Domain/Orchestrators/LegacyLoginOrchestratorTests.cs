using System.Collections.Generic;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Orchestration;
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
        public void Test1()
        { }

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
