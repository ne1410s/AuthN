using System;
using System.Threading.Tasks;
using AuthN.Domain.Exceptions;
using AuthN.Domain.Models.Request;
using AuthN.Domain.Models.Storage;
using AuthN.Domain.Services.Security;
using AuthN.Domain.Services.Storage;
using AuthN.Domain.Services.Validation;
using Microsoft.Extensions.Configuration;

namespace AuthN.Domain.Services.Orchestration
{
    /// <inheritdoc cref="ILegacyLoginOrchestrator"/>
    public class LegacyLoginOrchestrator : ILegacyLoginOrchestrator
    {
        private readonly string jwtIssuer;
        private readonly string jwtSecret;
        private readonly int defaultTokenDurationSeconds;
        private readonly IItemValidator<LegacyLoginRequest> validator;
        private readonly IUserRepository userRepo;

        /// <summary>
        /// Initialises a new instance of the <see cref="LegacyLoginOrchestrator"/>
        /// class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="validator">The request validator.</param>
        /// <param name="userRepo">The user repository.</param>
        public LegacyLoginOrchestrator(
            IConfiguration config,
            IItemValidator<LegacyLoginRequest> validator,
            IUserRepository userRepo)
        {
            jwtIssuer = config["Tokens::Issuer"];
            jwtSecret = config["Tokens::Secret"];
            var defaultTokenSeconds = config["Tokens::DefTokenDurationSeconds"];
            defaultTokenDurationSeconds = int.Parse(defaultTokenSeconds);

            this.validator = validator;
            this.userRepo = userRepo;
        }

        /// <inheritdoc/>
        public async Task<LoginSuccess> LegacyLoginAsync(
            LegacyLoginRequest request)
        {
            validator.AssertValid(request);

            var user = await AssertUserMatch(request);
            AssertHashMatch(request.Password, user);

            var duration = request.Duration ?? defaultTokenDurationSeconds;
            return ToLoginResponse(user, duration, jwtIssuer, jwtSecret);
        }

        private async Task<AuthNUser> AssertUserMatch(
            LegacyLoginRequest request)
        {
            return (!string.IsNullOrWhiteSpace(request.Username)
                ? await userRepo.FindByUsernameAsync(request.Username)
                : await userRepo.FindByEmailAsync(request.Email!))
                    ?? throw new DataStateException("User not found");
        }

        private static void AssertHashMatch(string password, AuthNUser user)
        {
            var incoming = password.Hash(user.PasswordSalt);
            if (incoming != user.PasswordHash)
            {
                throw new OrchestrationException("Invalid password");
            }
        }

        private static LoginSuccess ToLoginResponse(
            AuthNUser user,
            int tokenDuration,
            string tokenIssuer,
            string tokenSecret)
        {
            var expiry = DateTime.Now.AddSeconds(tokenDuration);
            return new LoginSuccess
            {
                User = user,
                Token = user.CreateJwt(tokenDuration, tokenSecret, tokenIssuer),
                TokenExpiresOn = expiry,
            };
        }
    }
}
