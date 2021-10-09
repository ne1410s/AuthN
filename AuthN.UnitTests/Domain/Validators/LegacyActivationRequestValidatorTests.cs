using System;
using System.Collections.Generic;
using System.Linq;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Validation.Models;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Domain.Validators
{
    /// <summary>
    /// Tests for <see cref="LegacyActivationRequestValidator"/>
    /// </summary>
    public class LegacyActivationRequestValidatorTests
    {
        [Fact]
        public void AssertValid_ValidRequest_DoesNotError()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var validSubject = GetSubjectValidByDefault();

            // Act
            Action act = () => sut.AssertValid(validSubject);

            // Assert
            act.Should().NotThrow<ValidatorException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingUsername_ThrowsException(string username)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(username: username);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Username' must not be empty.");
        }

        [Theory]
        [InlineData(6, 5)]
        public void AssertValid_BadLengthUsername_ThrowsException(
            int ruleMinLength,
            int actualFieldLength)
        {
            // Arrange
            var sut = GetSutWithConfig(minUsernameLength: ruleMinLength);
            var username = new string('0', actualFieldLength);
            var invalidSubject = GetSubjectValidByDefault(username: username);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                $"'Username' must be between {ruleMinLength} and 50"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingEmail_ThrowsException(string email)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(email: email);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Email' must not be empty.");
        }

        [Theory]
        [InlineData(6, 5)]
        public void AssertValid_BadLengthEmail_ThrowsException(
            int ruleMinLength,
            int actualFieldLength)
        {
            // Arrange
            var sut = GetSutWithConfig(minUsernameLength: ruleMinLength);
            var email = new string('0', actualFieldLength);
            var invalidSubject = GetSubjectValidByDefault(email: email);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                $"'Email' must be between {ruleMinLength} and 512"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("email")]
        [InlineData("email@@domain")]
        [InlineData("email.domain")]
        public void AssertValid_BadEmail_ThrowsException(string badEmail)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(email: badEmail);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Email' is not a valid email address.");
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void AssertValid_BadCode_ThrowsException(string code)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(guidString: code);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Activation Code' must not be empty.");
        }

        private static LegacyActivationRequestValidator GetSutWithConfig(
            int minUsernameLength = 6,
            int minEmailLength = 6)
        {
            var config = new Dictionary<string, string>
            {
                { "Validation:MinUsernameLength", $"{minUsernameLength}" },
                { "Validation:MinEmailLength", $"{minEmailLength}" },
            };

            var stubConfig = config.Stub();
            return new LegacyActivationRequestValidator(stubConfig);
        }

        private static LegacyActivationRequest GetSubjectValidByDefault(
            string guidString = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
            string email = "bob@test.co",
            string username = "bobsmith")
        {
            return new()
            {
                ActivationCode = Guid.Parse(guidString),
                Email = email,
                Username = username,
            };
        }
    }
}
