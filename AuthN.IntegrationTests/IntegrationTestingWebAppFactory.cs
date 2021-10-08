﻿using System;
using System.Linq;
using AuthN.Api;
using AuthN.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RefactorThis.IntegrationTests
{
    public class IntegrationTestingWebAppFactory
        : WebApplicationFactory<Startup>
    {
        private readonly Action<AuthNDbContext>? seedAction;

        public IntegrationTestingWebAppFactory(
            Action<AuthNDbContext>? seedAction = null)
        {
            this.seedAction = seedAction;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d =>
                    d.ServiceType == typeof(DbContextOptions<AuthNDbContext>));

                services.Remove(descriptor);
                services.AddDbContext<AuthNDbContext>(options =>
                    options.UseSqlite("Data Source=integration-test.db"));

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
                db.Roles.RemoveRange(db.Roles);
                db.SaveChanges();

                seedAction?.Invoke(db);
                db.SaveChanges();
            });
        }
    }
}
