using System.Threading.Tasks;
using AuthN.Domain.Models.Storage;
using AuthN.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace AuthN.UnitTests.Persistence.Repositories
{
    /// <summary>
    /// Tests for the <see cref="EfRoleRepository"/>.
    /// </summary>
    public class EfRoleRepositoryTests
    {
        [Fact]
        public async Task ListAllAsync_NonePresent_ReturnsEmptyList()
        {
            // Arrange
            var db = DbUtils.SeedSqlite();
            var sut = new EfRoleRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ListAllAsync_SomePresent_ReturnsItems()
        {
            // Arrange
            var existingRoles = new[]
            {
                new AuthNRole { Name = "role1" },
                new AuthNRole { Name = "role2" },
            };
            var db = DbUtils.SeedSqlite(s => s.Roles.AddRange(existingRoles));
            var sut = new EfRoleRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.Count.Should().Be(existingRoles.Length);
        }

        [Fact]
        public async Task ListAllAsync_ManyPresent_SortsAlphabetically()
        {
            // Arrange
            var roleB = new AuthNRole { Name = "bbb" };
            var roleA = new AuthNRole { Name = "aaa" };
            var roleC = new AuthNRole { Name = "ccc" };
            var db = DbUtils.SeedSqlite(s =>
                s.Roles.AddRange(roleB, roleA, roleC));
            var sut = new EfRoleRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result[0].Name.Should().Be(roleA.Name);
            result[1].Name.Should().Be(roleB.Name);
            result[2].Name.Should().Be(roleC.Name);
        }
    }
}