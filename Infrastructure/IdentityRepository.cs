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
        public IdentityRepository(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
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
    }
}
