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




        private static RoleValidator GetSutWithConfig()
        {
            var config = new Dictionary<string, string>();
            var stubConfig = config.Stub();
            return new RoleValidator(stubConfig);
        }

        private static AuthNRole GetSubjectValidByDefault(
            string roleName = "admin")
        {
            return new()
            {
                Name = roleName,
            };
        }
    }
}
