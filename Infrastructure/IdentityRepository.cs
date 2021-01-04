using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Core.Domain;

using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class IdentityRepository : IIdentityRepository
    {

        private readonly UserManager<User> _userManager;
        private readonly TokenController _tokenController;
        private SignInManager<User> _signInManager;
        public IdentityRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenController = new TokenController(configuration);
        }

        async public Task<JwtSecurityToken> Create(User user, string password)
        {
          
            List<Exception> exceptions = new List<Exception>();

            var result = await _userManager.CreateAsync(user, password);


            foreach (IdentityError error  in result.Errors)
            {
                exceptions.Add(new Exception(error.Description));
            }
            if (exceptions.Count >= 1)
            {
                throw new AggregateException("Encountered errors while trying to do something.",exceptions);
            }
            JwtSecurityToken token =  _tokenController.GenerateToken(user, null);
            return token;
        }

        public async Task<JwtSecurityToken> login(User user, string password)
        {

            var result = await _userManager.FindByEmailAsync(user.Email);

           
                if (result != null)
                {
                    var resultlogin = await _signInManager.PasswordSignInAsync(user.Email, password, false, false);
                    if (resultlogin.Succeeded)
                    {
                        JwtSecurityToken token = _tokenController.GenerateToken(user, null);
                        return token;
                    }
                    else
                    {
                        throw new Exception("Wrong password");
                    }
                }
                else
                {
                    throw new Exception("User doesnt exist");
                }
            
      
        }
    }
}
