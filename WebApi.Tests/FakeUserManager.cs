using Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Tests
{
    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(
                  new Mock<IUserStore<User>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<User>>().Object,
                  new IUserValidator<User>[0],
                  new IPasswordValidator<User>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<User>>>().Object)
        { }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }
        public Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }
        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult<bool>(!String.IsNullOrEmpty(user.PasswordHash));
        }

    }
}
