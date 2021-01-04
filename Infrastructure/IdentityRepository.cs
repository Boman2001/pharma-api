using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Core.Domain;
using Core.DomainServices.Helper;
using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class IdentityRepository : IIdentityRepository
    {

        private readonly UserManager<User> _userManager;
        private readonly AuthHelper _authHelper;
        private readonly SignInManager<User> _signInManager;
        public IdentityRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authHelper = new AuthHelper(configuration);
        }

        async public Task<JwtSecurityToken> Create(User user, string password)
        {
            _authHelper.IsValidEmail(user.Email);


            var result = await _userManager.CreateAsync(user, password);
            errorHandling(result);
            await _userManager.AddToRoleAsync(user, "Doctor");
            IList<String> roleslist = await _userManager.GetRolesAsync(user);
            
            JwtSecurityToken token = _authHelper.GenerateToken(user, roleslist);
            return token;
        }

        public async Task<JwtSecurityToken> login(User user, string password)
        {
            _authHelper.IsValidEmail(user.Email);
            var result = await _userManager.FindByEmailAsync(user.Email);

            //if(result == null) { throw new Exception("User doesnt exist");  }
            //if(password == null) { throw new Exception("Password not given");  }

            SignInResult resultlogin = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            IList<String> roleslist = await _userManager.GetRolesAsync(result);

            //iets beters voor dit bedenken? (HIERDONER)
            if (resultlogin.Succeeded) {
                JwtSecurityToken token = _authHelper.GenerateToken(user, roleslist);
                return token;
            } else {
               throw new Exception("Wrong password");
            }
        }

        public void errorHandling(IdentityResult result)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (IdentityError error in result.Errors)
            {
                exceptions.Add(new Exception(error.Description));
            }
            if (exceptions.Count >= 1)
            {
                throw new AggregateException("Encountered errors while trying to do something.", exceptions);
            }
        }


     


    }
}
