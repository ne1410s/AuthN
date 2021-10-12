using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;
using AuthN.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Persistence.Repositories
{
    /// <summary>
    /// Tests for the <see cref="EfPrivilegeRepository"/>.
    /// </summary>
    [Collection("Sequential")]
    public class EfPrivilegeRepositoryTests
    {
        [Fact]
        public async Task ListAllAsync_NonePresent_ReturnsEmptyList()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfPrivilegeRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ListAllAsync_SomePresent_ReturnsItems()
        {
            // Arrange
            var existingPrivileges = new[]
            {
                new AuthNPrivilege { Type = PrivilegeType.DeleteUser },
            };
            var db = DbUtils.SeedSqlite(s =>
                s.Privileges.AddRange(existingPrivileges));
            var sut = new EfPrivilegeRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.Count.Should().Be(existingPrivileges.Length);
        }

        [Fact]
        public async Task ListAllAsync_ManyPresent_SortsAlphabetically()
        {
            // Arrange
            var privA = new AuthNPrivilege { Type = PrivilegeType.Default };
            var privB = new AuthNPrivilege
            {
                Type = PrivilegeType.AssignPrivileges
            };
            var privC = new AuthNPrivilege { Type = PrivilegeType.DeleteUser };
            var db = DbUtils.SeedSqlite(s =>
                s.Privileges.AddRange(privA, privB, privC));
            var sut = new EfPrivilegeRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result[0].Type.Should().Be(privB.Type);
            result[1].Type.Should().Be(privA.Type);
            result[2].Type.Should().Be(privC.Type);
        }
    }
}