using System;
using System.Collections.Generic;
using System.Linq;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Validation.Models;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Domain.Validators
{
    /// <summary>
    /// Tests for <see cref="UserValidator"/>
    /// </summary>
    public class UserValidatorTests
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
            messages.Should().Contain(
                "'Registered Email' must not be empty.");
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
                $"'Registered Email' must be between {ruleMinLength} and 512"));
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
            messages.Should().Contain(
                "'Registered Email' is not a valid email address.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingSalt_ThrowsException(string salt)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(passwordSalt: salt);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Password Salt' must not be empty.");
        }

        [Fact]
        public void AssertValid_BadLengthSalt_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(passwordSalt: "a");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                "'Password Salt' must be between 32 and 512"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AssertValid_MissingHash_ThrowsException(string hash)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(passwordHash: hash);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Password Hash' must not be empty.");
        }

        [Fact]
        public void AssertValid_BadLengthHash_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(passwordHash: "a");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                "'Password Hash' must be between 32 and 512"));
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

        [Fact]
        public void AssertValid_BadCreatedOn_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var badDate = default(DateTime);
            var invalidSubject = GetSubjectValidByDefault(createdOn: badDate);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(
                "'Created On' must not be empty.");
        }

        private static UserValidator GetSutWithConfig(
            int minUsernameLength = 6,
            int minEmailLength = 6)
        {
            var config = new Dictionary<string, string>
            {
                { "Validation:MinUsernameLength", $"{minUsernameLength}" },
                { "Validation:MinEmailLength", $"{minEmailLength}" },
            };

            var stubConfig = config.Stub();
            return new UserValidator(stubConfig);
        }

        private static AuthNUser GetSubjectValidByDefault(
            string email = "bob@test.co",
            string username = "bobsmith",
            string passwordSalt = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            string passwordHash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            string forename = "Bob",
            string surname = "Smith",
            DateTime? createdOn = null)
        {
            return new()
            {
                Username = username,
                RegisteredEmail = email,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                Forename = forename,
                Surname = surname,
                CreatedOn = createdOn ?? DateTime.Today,
            };
        }
    }
}
