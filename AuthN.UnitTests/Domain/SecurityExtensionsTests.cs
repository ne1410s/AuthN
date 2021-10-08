using AuthN.Domain.Services.Security;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Domain
{
    public class SecurityExtensionsTests
    {
        private const string NaiveBase64Regex = @"^[A-Za-z0-9\/+]+={0,3}$";

        [Theory]
        [InlineData("text", "salt")]
        [InlineData("text", null)]
        [InlineData(null, "salt")]
        [InlineData(null, null)]
        public void Hash_AnyInputs_ProduceBase64(string payload, string? salt)
        {
            // Arrange
            // Act
            var result = payload.Hash(salt);

            // Assert
            result.Should().MatchRegex(NaiveBase64Regex);
        }
    }
}
