using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using System.Threading;

namespace Core.DomainServices.Helpers
{
    public class UserDomainValidator<TUser> : IUserValidator<TUser>
       where TUser : IdentityUser
    {
        List<IdentityError> Errors = new List<IdentityError>();


        public Task<IdentityResult> ValidateAsync(IdentityUser user)
        {
           
            
            return Task.FromResult(IdentityResult.Failed());
        }

        public Task<Microsoft.AspNetCore.Identity.IdentityResult> ValidateAsync(Microsoft.AspNetCore.Identity.UserManager<TUser> manager, TUser user)
        {
            var Result = manager.FindByEmailAsync(user.Email);
           
            if (Result.Result != null)
            {
                IdentityError inuse = new IdentityError();
                inuse.Description = "Mail Already in use";
                Errors.Add(inuse);
            }


            if (Errors.Count >= 1)
            {
                return Task.FromResult(IdentityResult.Failed(Errors.ToArray()));
            }
            else
            {
                return Task.FromResult(IdentityResult.Success);
            }

        }
    }
}