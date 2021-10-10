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
    /// Tests for <see cref="LegacyRegistrationRequestValidator"/>
    /// </summary>
    public class LegacyRegistrationRequestValidatorTests
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingForename_ThrowsException(string forename)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(forename: forename);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Forename' must not be empty.");
        }

        [Fact]
        public void AssertValid_BadLengthForename_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(forename: "a");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                "'Forename' must be between 2 and 50"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingSurname_ThrowsException(string surname)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(surname: surname);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Surname' must not be empty.");
        }

        [Fact]
        public void AssertValid_BadLengthSurname_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(surname: "a");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                "'Surname' must be between 2 and 50"));
        }

        private static LegacyRegistrationRequestValidator GetSutWithConfig(
            int minUsernameLength = 6,
            int minEmailLength = 6,
            int minPasswordLength = 8)
        {
            var config = new Dictionary<string, string>
            {
                { "Validation:MinUsernameLength", $"{minUsernameLength}" },
                { "Validation:MinEmailLength", $"{minEmailLength}" },
                { "Validation:MinPasswordLength", $"{minPasswordLength}" },
            };

            var stubConfig = config.Stub();
            return new LegacyRegistrationRequestValidator(stubConfig);
        }

        private static LegacyRegistrationRequest GetSubjectValidByDefault(
            string forename = "Bob",
            string surname = "Smith",
            string email = "bob@test.co",
            string username = "bobsmith",
            string password = "Aa!00000000")
        {
            return new()
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                Username = username,
                Password = password,
            };
        }
    }
}
