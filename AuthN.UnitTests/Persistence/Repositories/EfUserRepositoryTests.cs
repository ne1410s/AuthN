using System;
using System.Linq;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Storage;
using AuthN.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthN.UnitTests.Persistence.Repositories
{
    /// <summary>
    /// Tests for the <see cref="EfUserRepository"/>.
    /// </summary>
    [Collection("Sequential")]
    public class EfUserRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ValidUser_Success()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfUserRepository(db);
            var validUser = CreateUserValidByDefault();

            // Act
            await sut.AddAsync(validUser);

            // Assert
            var dbUser = db.Users.SingleOrDefault(u =>
                u.Username == validUser.Username);
            dbUser.Should().NotBeNull();
        }

        [Fact]
        public async Task AddAsync_MissingRequiredField_ThrowsException()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfUserRepository(db);
            var invalidUser = CreateUserValidByDefault(email: null!);

            // Act
            Func<Task> act = () => sut.AddAsync(invalidUser);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task AddAsync_DuplicatesUniqueValue_ThrowsException()
        {
            // Arrange
            static AuthNUser userGen() => CreateUserValidByDefault();
            var db = DbUtils.SeedSqlite(s => s.Users.Add(userGen()));
            var sut = new EfUserRepository(db);

            // Act
            Func<Task> act = () => sut.AddAsync(userGen());

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task ActivateAsync_ExistingUnactivated_UpdatesActivation()
        {
            // Arrange
            var user = CreateUserValidByDefault();
            var db = DbUtils.SeedSqlite(s => s.Users.Add(user));
            var sut = new EfUserRepository(db);

            // Act
            await sut.ActivateAsync(user.Username);

            // Assert
            var dbUser = db.Users.Single(u => u.Username == user.Username);
            dbUser.ActivatedOn.Should().NotBeNull();
        }

        [Fact]
        public async Task ActivateAsync_AlreadyActive_NotUpdated()
        {
            // Arrange
            var activated = new DateTime(2015, 10, 26);
            var user = CreateUserValidByDefault(activated: activated);
            var db = DbUtils.SeedSqlite(s => s.Users.Add(user));
            var sut = new EfUserRepository(db);

            // Act
            await sut.ActivateAsync(user.Username);

            // Assert
            var dbUser = db.Users.Single(u => u.Username == user.Username);
            dbUser.ActivatedOn.Should().BeSameDateAs(activated);
        }

        [Fact]
        public async Task ActivateAsync_NoSuchUser_ThrowsException()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfUserRepository(db);

            // Act
            Func<Task> act = () => sut.ActivateAsync("missing");

            // Assert
            await act.Should()
                .ThrowAsync<DataStateException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task FindByEmailAsync_NoMatch_ReturnsNull()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfUserRepository(db);

            // Act
            var result = await sut.FindByEmailAsync("missing");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByEmailAsync_IsMatch_ReturnsUser()
        {
            // Arrange
            var existingUser = CreateUserValidByDefault();
            var db = DbUtils.SeedSqlite(s => s.Users.Add(existingUser));
            var sut = new EfUserRepository(db);

            // Act
            var result = await sut.FindByEmailAsync(
                existingUser.RegisteredEmail);

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be(existingUser.Username);
        }

        [Fact]
        public async Task FindByUsernameAsync_NoMatch_ReturnsNull()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfUserRepository(db);

            // Act
            var result = await sut.FindByUsernameAsync("missing");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByUsernameAsync_IsMatch_ReturnsUser()
        {
            // Arrange
            var existingUser = CreateUserValidByDefault();
            var db = DbUtils.SeedSqlite(s => s.Users.Add(existingUser));
            var sut = new EfUserRepository(db);

            // Act
            var result = await sut.FindByUsernameAsync(existingUser.Username);

            // Assert
            result.Should().NotBeNull();
            result!.RegisteredEmail.Should().Be(existingUser.RegisteredEmail);
        }

        private static AuthNUser CreateUserValidByDefault(
            string username = "bobsmith",
            string email = "bob@test.co",
            DateTime? activated = null) => new()
        {
            Username = username,
            RegisteredEmail = email,
            Forename = string.Empty,
            Surname = string.Empty,
            PasswordSalt = string.Empty,
            PasswordHash = string.Empty,
            ActivatedOn = activated,
        };
    }
}
