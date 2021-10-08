using System;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Security;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Domain
{
    /// <summary>
    /// Tests for <see cref="SecurityExtensions"/>.
    /// </summary>
    public class SecurityExtensionsTests
    {
        private const string NaiveBase64Regex = @"^[A-Za-z0-9\/+]+={0,3}$";
        private const string NaiveJwtRegex = "^[A-Za-z0-9._-]+$";
        private const string ValidSigningKey = "jaboutlongenough";

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateJwt_BadIssuer_ThrowsException(string issuer)
        {
            // Arrange
            var user = new AuthNUser();

            // Act
            Action act = () => user.CreateJwt(1, "key", issuer);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Issuer is required");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("notquite16chars")]
        public void CreateJwt_BadKey_ThrowsException(string key)
        {
            // Arrange
            var user = new AuthNUser();

            // Act
            Action act = () => user.CreateJwt(1, key, "issuer");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Key must be >= 16 characters");
        }

        [Fact]
        public void CreateJwt_BadDuration_ThrowsException()
        {
            // Arrange
            var user = new AuthNUser();
            const uint badDuration = 0;

            // Act
            Action act = () =>
                user.CreateJwt(badDuration, ValidSigningKey, "issuer");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Duration must be > 0 seconds");
        }

        [Fact]
        public void CreateJwt_ValidRequest_ProducesExpectedPattern()
        {
            // Arrange
            var user = GetValidUser();

            // Act
            var result = user.CreateJwt(1, ValidSigningKey, "issuer");

            // Assert
            result.Should().MatchRegex(NaiveJwtRegex);
        }

        //TODO:


            // Check contents (roundtrip)

        private static AuthNUser GetValidUser() => new()
        {
            Username = "bobsmith",
            RegisteredEmail = "bob@test.co",
            Forename = "bob",
            Surname = "smith",
        };
    }
}
