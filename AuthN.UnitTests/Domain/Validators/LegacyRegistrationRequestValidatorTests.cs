using System;
using System.Collections.Generic;
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

        // TODO: Moar tests!!


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
