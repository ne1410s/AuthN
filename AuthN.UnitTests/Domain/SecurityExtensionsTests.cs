using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
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

        [Fact]
        public void CreateJwt_WithFullClaims_ParsesOk()
        {
            // Arrange
            var user = GetValidUser("role1", "role2");
            const string issuer = "issuer";

            // Act
            var result = user.CreateJwt(60, ValidSigningKey, issuer);

            // Assert
            var jwtHandler = new JwtSecurityTokenHandler();
            var parsedJwt = jwtHandler.ReadJwtToken(result);
            parsedJwt.Issuer.Should().Be(issuer);
            parsedJwt.Subject.Should().Be(user.Username);
            GetClaim(parsedJwt, JwtRegisteredClaimNames.Email)
                .Should().Be(user.RegisteredEmail);
            GetClaim(parsedJwt, JwtRegisteredClaimNames.GivenName)
                .Should().Be(user.Forename);
            GetClaim(parsedJwt, JwtRegisteredClaimNames.FamilyName)
                .Should().Be(user.Surname);
            GetClaim(parsedJwt, JwtRegisteredClaimNames.Jti)
                .Should().NotBeNullOrWhiteSpace();
            var rolesJson = GetClaim(parsedJwt, "Roles")!;
            var roles = JsonSerializer.Deserialize<string[]>(rolesJson);
            roles.Should().Contain(user.Roles[0].Name);
            roles.Should().Contain(user.Roles[1].Name);
        }

        [Fact]
        public void Tokenise_ValidInput_ReturnsResult()
        {
            // Arrange
            var user = GetValidUser();

            // Act
            var result = user.Tokenise(10, "issuer", ValidSigningKey);

            // Assert
            result.User.Should().Be(user);
            result.Token.Should().NotBeNullOrWhiteSpace();
            result.TokenExpiresOn.Should().BeAfter(DateTime.Now);
        }

        private static AuthNUser GetValidUser(params string[] roles) => new()
        {
            Username = "bobsmith",
            RegisteredEmail = "bob@test.co",
            Forename = "bob",
            Surname = "smith",
            Roles = roles.Select(r => new AuthNRole { Name = r }).ToList()
        };

        private static string? GetClaim(JwtSecurityToken jwt, string name)
            => jwt.Claims.SingleOrDefault(c => c.Type == name)?.Value;
    }
}
