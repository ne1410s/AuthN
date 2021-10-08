using System.Linq;
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
            var db = await DbUtils.SeedSqliteAync();
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
            var db = await DbUtils.SeedSqliteAync(s =>
                s.Roles.AddRange(existingRoles));
            var sut = new EfRoleRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.Count.Should().Be(existingRoles.Count());
        }

        [Fact]
        public async Task ListAllAsync_ManyPresent_SortsAlphabetically()
        {
            // Arrange
            var roleB = new AuthNRole { Name = "bbb" };
            var db = await DbUtils.SeedSqliteAync(s => s.Roles.Add(roleB));
            var roleA = new AuthNRole { Name = "aaa" };
            db.Roles.Add(roleA);
            await db.SaveChangesAsync();
            var roleC = new AuthNRole { Name = "ccc" };
            db.Roles.Add(roleC);
            await db.SaveChangesAsync();
            var sut = new EfRoleRepository(db);

            // Act
            var result = await sut.ListAllAsync();

            // Assert
            result.ElementAt(0).Name.Should().Be(roleA.Name);
            result.ElementAt(1).Name.Should().Be(roleB.Name);
            result.ElementAt(2).Name.Should().Be(roleC.Name);
        }
    }
}