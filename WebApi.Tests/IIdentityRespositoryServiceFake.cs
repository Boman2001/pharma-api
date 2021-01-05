using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace WebApi.Tests
{
    class IIdentityRespositoryServiceFake : IIdentityRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthHelper _authHelper;
        private readonly SignInManager<User> _signInManager;
        public IIdentityRespositoryServiceFake(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authHelper = new AuthHelper(configuration);

        }
        async public Task<JwtSecurityToken> Create(User user, string password)
        {

            if (password == null) { throw new Exception("Password not given"); }
            if (user.Email == null) { throw new Exception("Email not given"); }
            _authHelper.IsValidEmail(user.Email);
            IList<String> roleslist = new List<String>();
            roleslist.Add("DOCTOR");

            JwtSecurityToken token = _authHelper.GenerateToken(user, roleslist);
            return token;
        }

        async public  Task<JwtSecurityToken> login(User user, string password)
        {
            _authHelper.IsValidEmail(user.Email);
            if (password == null) { throw new Exception("Password not given"); }
            if (user.Email == null) { throw new Exception("Email not given"); }
            if (user.Email != "email@gmail.com") { throw new Exception("User doesnt exist"); }
            if(password != "bijen") { throw new Exception("Wrong password"); }

            IList<String> roleslist = new List<String>();
            roleslist.Add("DOCTOR");
                JwtSecurityToken token = _authHelper.GenerateToken(user, roleslist);
                return token;
        }


    }
}
