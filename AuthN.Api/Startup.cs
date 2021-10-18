using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuthN.Api.Middleware;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Orchestration.LegacyWorkflow;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using AuthN.Domain.Services.Validation.Models;
using AuthN.Persistence;
using AuthN.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AuthN.Api
{
    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The host environment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the host environment.
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Called by the runtime to add services to the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AuthN.Api",
                    Version = "v1"
                });
            });

            services.Configure<ApiBehaviorOptions>(opts =>
            {
                opts.InvalidModelStateResponseFactory = ctx =>
                {
                    var invalidFields = ctx.ModelState
                        .Where(e => e.Value.Errors.Count != 0)
                        .Select(e =>
                        {
                            var errors = e.Value.Errors
                                .Select(m => m.ErrorMessage);
                            var fieldMessage = string.Join(", ", errors);
                            return new InvalidItem
                            {
                                ErrorMessage = fieldMessage,
                                AttemptedValue = e.Value.RawValue,
                                Property = e.Key,
                            };
                        });

                    throw new ValidatorException(invalidFields.ToArray());
                };
            });

            var connectionString = Configuration.GetConnectionString("AuthNDb");
            services.AddDbContext<AuthNDbContext>(
                options => options.UseSqlServer(connectionString));

            services.AddHttpClient();

            InjectOrchestrators(services);
            InjectValidators(services);
            InjectRepositories(services);
        }

        /// <summary>
        /// Called by the runtime to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The host environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "AuthN.Api v1");

                // Disable all calls via UI
                c.SupportedSubmitMethods(Array.Empty<SubmitMethod>());
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionHandler>();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static void InjectOrchestrators(IServiceCollection services)
        {
            services.AddTransient<
                ILegacyRegistrationOrchestrator,
                LegacyRegistrationOrchestrator>();
            services.AddTransient<
                ILegacyActivationOrchestrator,
                LegacyActivationOrchestrator>();
            services.AddTransient<
                ILegacyLoginOrchestrator,
                LegacyLoginOrchestrator>();
        }

        private static void InjectValidators(IServiceCollection services)
        {
            services.AddTransient<IItemValidator<AuthNUser>, UserValidator>();
            services.AddTransient<
                IItemValidator<AuthNPrivilege>,
                PrivilegeValidator>();

            services.AddTransient<
                IItemValidator<LegacyRegistrationRequest>,
                LegacyRegistrationRequestValidator>();
            services.AddTransient<
                IItemValidator<LegacyActivationRequest>,
                LegacyActivationRequestValidator>();
            services.AddTransient<
                IItemValidator<LegacyLoginRequest>,
                LegacyLoginRequestValidator>();
        }

        private static void InjectRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, EfUserRepository>();
        }
    }
}
