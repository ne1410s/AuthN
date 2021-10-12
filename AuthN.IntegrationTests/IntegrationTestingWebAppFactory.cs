using System;
using System.IO;
using System.Linq;
using AuthN.Api;
using AuthN.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthN.IntegrationTests
{
    public class IntegrationTestingWebAppFactory
        : WebApplicationFactory<Startup>
    {
        private const string DbFileName = "integration-test.db";

        private readonly Action<AuthNDbContext>? seedAction;
        private readonly Action<IServiceCollection>? servicesAction;

        public IntegrationTestingWebAppFactory(
            Action<AuthNDbContext>? seedAction = null,
            Action<IServiceCollection>? servicesAction = null)
        {
            this.seedAction = seedAction;
            this.servicesAction = servicesAction;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("https_port", "5001");

            File.Delete(DbFileName);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d =>
                    d.ServiceType == typeof(DbContextOptions<AuthNDbContext>));

                services.Remove(descriptor);
                services.AddDbContext<AuthNDbContext>(options =>
                    options.UseSqlite($"Data Source={DbFileName}"));

                servicesAction?.Invoke(services);

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AuthNDbContext>();
                db.Database.OpenConnection();

                // This method provides an up-to-date schema without applying
                // migrations meaning the sql technology is interchangeable
                // (provided there is no vendor-specific fluent entity config).
                db.Database.EnsureCreated();

                db.Users.RemoveRange(db.Users);
                db.Privileges.RemoveRange(db.Privileges);
                db.SaveChanges();

                seedAction?.Invoke(db);
                db.SaveChanges();
            });
        }
    }
}
