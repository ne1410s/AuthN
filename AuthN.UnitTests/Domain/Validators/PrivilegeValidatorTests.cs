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
    /// Tests for <see cref="PrivilegeValidator"/>
    /// </summary>
    public class PrivilegeValidatorTests
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
        [InlineData((PrivilegeType)(-1))]
        [InlineData((PrivilegeType)(2349984))]
        public void AssertValid_PrivilegeNotInEnum_ThrowsException(
            PrivilegeType privilege)
        {
            // Arrange
            var sut = GetSutWithConfig();
            var invalidSubject = GetSubjectValidByDefault(privilege);

            // Act
            Action act = () => sut.AssertValid(invalidSubject);

            // Assert
            var ex = act.Should().Throw<ValidatorException>().Which;
            var messages = ex.InvalidItems.Select(item => item.ErrorMessage);
            messages.Should().Contain(m => m.EndsWith(
                $"range of values which does not include '{(int)privilege}'."));
        }

        private static PrivilegeValidator GetSutWithConfig()
        {
            var config = new Dictionary<string, string>();
            var stubConfig = config.Stub();
            return new PrivilegeValidator(stubConfig);
        }

        private static AuthNPrivilege GetSubjectValidByDefault(
            PrivilegeType privilege = PrivilegeType.DeleteUser)
        {
            return new()
            {
                Type = privilege,
            };
        }
    }
}
