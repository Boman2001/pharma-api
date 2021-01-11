using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace WebApi.Tests.Mocks
{
    public static class MockSigninManager
    {
        public static Mock<SignInManager<TUser>> GetSignInManager<TUser>(UserManager<IdentityUser> userManager) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>().Object;
            var claimsPrincipal = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>().Object;
            var iLogger = new Mock<ILogger<SignInManager<IdentityUser>>>().Object;
            var isAuthenticatedAuthenticationSchemeProvider = new Mock<IAuthenticationSchemeProvider>().Object;
            var isUserConfirmation = new Mock<IUserConfirmation<IdentityUser>>().Object;

            var mgr = new Mock<SignInManager<TUser>>(
                userManager,
                contextAccessor,
                claimsPrincipal,
                options,
                iLogger,
                isAuthenticatedAuthenticationSchemeProvider,
                isUserConfirmation);

            mgr.Setup(
                x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                    It.IsAny<bool>())).ReturnsAsync(SignInResult.Success).Verifiable();

            return mgr;
        }

        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            using var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8);
            var salt = bytes.Salt;
            var buffer2 = bytes.GetBytes(0x20);
            var dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
    }
}
