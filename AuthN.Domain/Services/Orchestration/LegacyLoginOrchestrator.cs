using System;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;

namespace AuthN.Domain.Services.Orchestration
{
    /// <inheritdoc cref="ILegacyLoginOrchestrator"/>
    public class LegacyLoginOrchestrator : ILegacyLoginOrchestrator
    {
        private static readonly TimeSpan DefaultDuration
            = TimeSpan.FromSeconds(600);

        private readonly IItemValidator<LegacyLoginRequest> validator;
        private readonly IUserRepository userRepo;

        /// <summary>
        /// Initialises a new instance of the <see cref="LegacyLoginOrchestrator"/>
        /// class.
        /// </summary>
        /// <param name="validator">The request validator.</param>
        /// <param name="userRepo">The user repository.</param>
        public LegacyLoginOrchestrator(
            IItemValidator<LegacyLoginRequest> validator,
            IUserRepository userRepo)
        {
            this.validator = validator;
            this.userRepo = userRepo;
        }

        /// <inheritdoc/>
        public async Task<LoginSuccess> LegacyLoginAsync(
            LegacyLoginRequest request)
        {
            validator.AssertValid(request);
            var user = await FindUser(request)
                ?? throw new DataStateException("User not found");


        }

        private async Task<AuthNUser?> FindUser(LegacyLoginRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Username)
                ? await userRepo.FindByUsernameAsync(request.Username)
                : await userRepo.FindByEmailAsync(request.Email!);
        }

        private LoginSuccess MapToResponse(
            AuthNUser user,
            LegacyLoginRequest request)
        {
            var expiry = DateTime.Now.AddSeconds(
                request.Duration ?? DefaultDuration.TotalSeconds);

            return new LoginSuccess
            {
                User = user,
                Token = "", //TODO!
                TokenExpiresOn = expiry,
            };
        }
    }
}
