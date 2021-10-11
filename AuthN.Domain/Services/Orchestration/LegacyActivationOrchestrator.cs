using System;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Orchestration
{
    /// <inheritdoc cref="ILegacyActivationOrchestrator"/>
    public class LegacyActivationOrchestrator : ILegacyActivationOrchestrator
    {
        private readonly TimeSpan activationWindow;
        private readonly IItemValidator<LegacyActivationRequest> validator;
        private readonly IUserRepository userRepo;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyActivationOrchestrator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="validator">The request validator.</param>
        /// <param name="userRepo">The user repository.</param>
        public LegacyActivationOrchestrator(
            IConfiguration config,
            IItemValidator<LegacyActivationRequest> validator,
            IUserRepository userRepo)
        {
            var windowHours = config["LegacyAuth:ActivationWindowHours"];
            activationWindow = TimeSpan.FromHours(double.Parse(windowHours));

            this.validator = validator;
            this.userRepo = userRepo;
        }

        /// <inheritdoc/>
        public async Task LegacyActivateAsync(
            LegacyActivationRequest request)
        {
            validator.AssertValid(request);
            var user = await userRepo.FindByUsernameAsync(request.Username);

            if (user?.ActivationCode != request.ActivationCode
                || user?.ActivationCodeGeneratedOn == null
                || request.Email != user.RegisteredEmail)
            {
                throw new DataStateException("No matching users found.");
            }

            if (user.ActivatedOn != null)
            {
                throw new OrchestrationException("User is already activated.");
            }

            var earliestCodeGeneratedOn = DateTime.UtcNow - activationWindow;
            if (user.ActivationCodeGeneratedOn < earliestCodeGeneratedOn)
            {
                throw new OrchestrationException("Activation code expired.");
            }

            await userRepo.ActivateAsync(user.Username);
        }
    }
}
