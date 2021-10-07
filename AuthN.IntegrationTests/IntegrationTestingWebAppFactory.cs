//using System;
//using System.Linq;
//using Afi.Registration.Api;
//using Afi.Registration.Persistence;
//using AuthN.Api;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace RefactorThis.IntegrationTests
//{
//    public class IntegrationTestingWebAppFactory : WebApplicationFactory<Startup>
//    {
//        private readonly Action<AuthNDbContext> seedAction;

//        public IntegrationTestingWebAppFactory(
//            Action<AuthNDbContext> seedAction = null)
//        {
//            this.seedAction = seedAction;
//        }

//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices(services =>
//            {
//                var descriptor = services.SingleOrDefault(
//                    d => d.ServiceType == typeof(DbContextOptions<AuthNDbContext>));

//                services.Remove(descriptor);
//                services.AddDbContext<AuthNDbContext>(options =>
//                {
//                    options.UseSqlServer("soem in-mem solution!");
//                });

//                var sp = services.BuildServiceProvider();
//                using var scope = sp.CreateScope();

//                var scopedServices = scope.ServiceProvider;
//                var db = scopedServices.GetRequiredService<AuthNDbContext>();
//                db.Database.OpenConnection();
//                db.Database.EnsureCreated();
                
//                //db.Customers.RemoveRange(db.Customers);
//                //db.Policies.RemoveRange(db.Policies);
//                db.SaveChanges();

//                seedAction?.Invoke(db);
//                db.SaveChanges();
//            });
//        }
//    }
//}
