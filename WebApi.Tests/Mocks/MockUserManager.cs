using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace WebApi.Tests.Mocks
{
    public static class MockUserManager
    {
        public static Mock<UserManager<IdentityUser>> GetMockUserManager(List<IdentityUser> ls)
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, new PasswordHasher<IdentityUser>(), null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<IdentityUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<IdentityUser>());


            mgr.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success).Callback<IdentityUser>(x => { ls.RemoveAll(a => a.Id == x.Id); });

            mgr.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<IdentityUser, string>((x, y) =>
                {
                    x.PasswordHash = y;
                    ls.Add(x);
                });

            mgr.Setup(x => x.UpdateAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string b) => ls.FirstOrDefault(f => f.Email == b));

            mgr.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string b) => ls.FirstOrDefault(f => f.Id == b));

            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true
                );

           mgr.Setup(x => x.GetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
               .ReturnsAsync((string b) => "value");
            mgr.Setup(x => x.Users).Returns(ls.AsQueryable());

            mgr.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync((IdentityUser user) => new List<string>() {"Admin"}.ToList());

            return mgr;
        }
    }
}