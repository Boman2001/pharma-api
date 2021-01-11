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
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<IdentityUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<IdentityUser>());


            mgr.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success).Callback<IdentityUser>((x) =>
                {
                  ls.RemoveAll(a => a.Id == x.Id);
                });

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
            return mgr;
        }
    }
}
