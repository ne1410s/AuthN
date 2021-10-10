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
    /// Tests for <see cref="LegacyLoginRequestValidator"/>
    /// </summary>
    public class LegacyLoginRequestValidatorTests
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
        public void AssertValid_BadEmail_ThrowsException(string? badEmail)
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
        [InlineData("bobsmith", null)]
        [InlineData(null, "test@domain")]
        public void AssertValid_UsernameOrEmailNotBoth_DoesNotError(
            string? username,
            string? email)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(
                username: username,
                email: email);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().NotThrow<ValidatorException>();
        }

        [Fact]
        public void AssertValid_NoUsernameOrEmail_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(
                username: null,
                email: null);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "Username or email must be provided.");
        }

        [Fact]
        public void AssertValid_UsernameAndEmail_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(
                username: "bobsmith",
                email: "test@domain");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "Username or email must be provided, not both.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingPassword_ThrowsException(string password)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(password: password);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Password' must not be empty.");
        }

        [Fact]
        public void AssertValid_SimplePasswordNoUpperCase_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            const string invalidPassword = "a";
            var invalidSubject = GetSubjectValidByDefault(
                password: invalidPassword);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "'Password' must contain at least one upper case letter.");
        }

        [Fact]
        public void AssertValid_SimplePasswordNoLowerCase_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            const string invalidPassword = "A";
            var invalidSubject = GetSubjectValidByDefault(
                password: invalidPassword);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "'Password' must contain at least one lower case letter.");
        }

        [Fact]
        public void AssertValid_SimplePasswordNoNumber_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            const string invalidPassword = "a";
            var invalidSubject = GetSubjectValidByDefault(
                password: invalidPassword);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "'Password' must contain at least one number.");
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aB 12")]
        public void AssertValid_SimplePasswordNoSpecials_ThrowsException(
            string invalidPassword)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(
                password: invalidPassword);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "'Password' must contain at least one special character.");
        }

        [Theory]
        [InlineData(4)]
        [InlineData(11, 10)]
        public void AssertValid_BadRangeDuration_ThrowsException(
            int duration,
            int maxDuration = 600)
        {
            // Arrange
            var sut = GetSutWithConfig(maxTokenMinutes: maxDuration / 60d);
            var invalidSubject = GetSubjectValidByDefault(duration: duration);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                $"'Duration' must be between 5 and {maxDuration}."));
        }

        private static LegacyLoginRequestValidator GetSutWithConfig(
            int minUsernameLength = 6,
            int minEmailLength = 6,
            int minPasswordLength = 8,
            double maxTokenMinutes = 600)
        {
            var config = new Dictionary<string, string>
            {
                { "Validation:MinUsernameLength", $"{minUsernameLength}" },
                { "Validation:MinEmailLength", $"{minEmailLength}" },
                { "Validation:MinPasswordLength", $"{minPasswordLength}" },
                { "Validation:MaxTokenMinutes", $"{maxTokenMinutes}" },
            };

            var stubConfig = config.Stub();
            return new LegacyLoginRequestValidator(stubConfig);
        }

        private static LegacyLoginRequest GetSubjectValidByDefault(
            int duration = 60,
            string? email = null,
            string? username = "bobsmith",
            string password = "Aa!00000000")
        {
            return new()
            {
                Duration = duration,
                Email = email,
                Username = username,
                Password = password,
            };
        }
    }
}
