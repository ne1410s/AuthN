using System;
using AuthN.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthN.UnitTests.Persistence.Repositories
{
    /// <summary>
    /// Utility methods for working with entity framework database context.
    /// </summary>
    public static class DbUtils
    {
        /// <summary>
        /// Provides a legit sqlite database file with an up-to-date schema.
        /// </summary>
        /// <param name="seedAction">Used to seed any prerequisite data.</param>
        /// <returns>A database context.</returns>
        public static AuthNDbContext SeedSqlite(
            Action<AuthNDbContext>? seedAction = null)
        {
            var dbOptsBuilder = new DbContextOptionsBuilder<AuthNDbContext>();
            dbOptsBuilder.UseSqlite("Data Source=unit-test.db");

            var db = new AuthNDbContext(dbOptsBuilder.Options);
            db.Database.OpenConnection();

            // This method provides an up-to-date schema without applying any
            // migrations. This means that the sql technology is interchangeable
            // (provided that there is no vendor-specific fluent entity config).
            db.Database.EnsureCreated();

            db.Users.RemoveRange(db.Users);
            db.Privileges.RemoveRange(db.Privileges);
            db.SaveChanges();

            seedAction?.Invoke(db);
            db.SaveChanges();
            return db;
        }
    }
}
