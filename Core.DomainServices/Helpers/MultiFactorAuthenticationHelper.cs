using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Core.DomainServices.Helpers
{
    public class MultiFactorAuthenticationHelper
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly IIdentityRepository _identityRepository;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public MultiFactorAuthenticationHelper(
            UserManager<IdentityUser> userManager, IIdentityRepository identityRepository)
        {
            _userManager = userManager;
            _urlEncoder = UrlEncoder.Default;
            _identityRepository = identityRepository;
        }

        public async Task<string> LoadSharedKeyAndQrCodeUriAsync(IdentityUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            return await GenerateQrCodeUri(user, unformattedKey);
        }

        public async Task<string> FormatKey(IdentityUser user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private async Task<string> GenerateQrCodeUri(IdentityUser user, string unformattedKey)
        {
            var email = await _userManager.GetEmailAsync(user);
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("WebApi"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        public async Task<JwtSecurityToken> ValidateTwoFactor(IdentityUser user, string verificationCode)
        {
            if (await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode))
            {
                return await _identityRepository.GetTokenForTwoFactor(user);
            }
            else
            {
                throw new Exception("Ongeldige 2-fa");
            }
        }
    }
}