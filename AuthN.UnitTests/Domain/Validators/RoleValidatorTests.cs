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
    /// Tests for <see cref="RoleValidator"/>
    /// </summary>
    public class RoleValidatorTests
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
        public void AssertValid_MissingName_ThrowsException(string roleName)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(roleName: roleName);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Name' must not be empty.");
        }

        [Fact]
        public void AssertValid_BadLengthName_ThrowsException()
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(roleName: "a");

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.StartsWith(
                "'Name' must be between 4 and 30"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("Role")]
        [InlineData("bad role")]
        [InlineData("bad-Role")]
        public void AssertValid_BadName_ThrowsException(string badName)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(roleName: badName);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain("'Name' is not in the correct format.");
        }

        private static RoleValidator GetSutWithConfig()
        {
            var config = new Dictionary<string, string>();
            var stubConfig = config.Stub();
            return new RoleValidator(stubConfig);
        }

        private static AuthNRole GetSubjectValidByDefault(
            string roleName = "admin-role-test")
        {
            return new()
            {
                Name = roleName,
            };
        }
    }
}
