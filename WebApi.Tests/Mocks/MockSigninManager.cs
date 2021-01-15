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
        public static Mock<SignInManager<TUser>> GetSignInManager<TUser>(UserManager<IdentityUser> userManager)
            where TUser : class
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
    }
}