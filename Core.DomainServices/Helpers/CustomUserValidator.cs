using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using System.Threading;

namespace Core.DomainServices.Helpers
{
    public class UserDomainValidator<TUser> : IUserValidator<TUser>
       where TUser : IdentityUser
    {
        readonly List<IdentityError> _errors = new List<IdentityError>();


        public Task<IdentityResult> ValidateAsync(IdentityUser user)
        {
            return Task.FromResult(IdentityResult.Failed());
        }

        public Task<IdentityResult> ValidateAsync(Microsoft.AspNetCore.Identity.UserManager<TUser> manager, TUser user)
        {
            var resultByEmailAsync = manager.FindByEmailAsync(user.Email);
           
            if (resultByEmailAsync.Result != null)
            {
                IdentityError inuse = new IdentityError();
                inuse.Description = "Mail Already in use";
                _errors.Add(inuse);
            }


            if (_errors.Count >= 1)
            {
                return Task.FromResult(IdentityResult.Failed(_errors.ToArray()));
            }
            else
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}