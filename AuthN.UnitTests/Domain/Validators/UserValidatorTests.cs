using System;
using System.Collections.Generic;
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

        //TODO!!!


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
