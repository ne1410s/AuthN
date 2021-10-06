﻿using System;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;

namespace AuthN.Domain.Services.Orchestration
{
    /// <inheritdoc cref="ILegacyRegistrationOrchestrator"/>
    public class LegacyRegistrationOrchestrator
        : ILegacyRegistrationOrchestrator
    {
        private readonly IItemValidator<LegacyRegistrationRequest> validator;
        private readonly IUserRepository userRepo;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="LegacyRegistrationOrchestrator"/> class.
        /// </summary>
        /// <param name="validator">The request validator.</param>
        /// <param name="userRepo">The user respository.</param>
        public LegacyRegistrationOrchestrator(
            IItemValidator<LegacyRegistrationRequest> validator,
            IUserRepository userRepo)
        {
            this.validator = validator;
            this.userRepo = userRepo;
        }

        /// <inheritdoc/>
        public async Task<LegacyRegistrationSuccess> LegacyRegisterAsync(
            LegacyRegistrationRequest request)
        {
            validator.AssertValid(request);

            await AssertUniqueEmail(request);
            await AssertUniqueUsername(request);

            var user = MapToNewUser(request);
            await userRepo.AddAsync(user);

            return new() { ActivationCode = user.ActivationCode!.Value };
        }

        private async Task AssertUniqueEmail(
            LegacyRegistrationRequest request)
        {
            var emailCheck = await userRepo.FindByEmailAsync(request.Username);
            if (emailCheck != null)
            {
                var errorMessage = emailCheck.ActivatedOn == null
                    ? "This email is awaiting activation"
                    : "This email is taken";
                throw new DataStateException(errorMessage);
            }
        }
        private async Task AssertUniqueUsername(
            LegacyRegistrationRequest request)
        {
            var usrCheck = await userRepo.FindByUsernameAsync(request.Username);
            if (usrCheck != null)
            {
                var errorMessage = usrCheck.ActivatedOn == null
                    ? "This username is awaiting activation"
                    : "This username is taken";
                throw new DataStateException(errorMessage);
            }
        }

        private static AuthNUser MapToNewUser(LegacyRegistrationRequest request)
        {
            var utcNow = DateTime.UtcNow;
            return new AuthNUser
            {
                ActivationCode = Guid.NewGuid(),
                ActivationCodeGeneratedOn = utcNow,
                CreatedOn = utcNow,
                Forename = request.Forename,
                Surname = request.Surname,
                RegistrationEmail = request.Email.ToLower(),
                Username = request.Username, 
                // salt
                // hash
            };
        }
    }
}
