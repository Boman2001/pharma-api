using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.DomainServices.Helpers
{
    public class UserDomainValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
    {
        private readonly List<IdentityError> _errors = new();

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var resultByEmailAsync = manager.FindByEmailAsync(user.Email);

            if (resultByEmailAsync.Result != null)
            {
                var inuse = new IdentityError {Description = "Mail Already in use"};
                _errors.Add(inuse);
            }

            if (_errors.Count >= 1)
            {
                return Task.FromResult(IdentityResult.Failed(_errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> ValidateAsync(IdentityUser user)
        {
            return Task.FromResult(IdentityResult.Failed());
        }
    }
}