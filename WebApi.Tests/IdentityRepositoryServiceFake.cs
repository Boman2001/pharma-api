using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace WebApi.Tests
{
    class IdentityRepositoryServiceFake : IIdentityRepository
    {
        private readonly AuthHelper _authHelper;
        public IdentityRepositoryServiceFake(IConfiguration configuration)
        {
            _authHelper = new AuthHelper(configuration);
        }
        async public Task<JwtSecurityToken> Register(IdentityUser user, string password)
        {

            if (password == null) { throw new Exception("Password not given"); }
            if (user.Email == null) { throw new Exception("Incorrect email"); }
            _authHelper.IsValidEmail(user.Email);
            IList<String> RolesList = new List<String>
            {
                "DOCTOR"
            };

            JwtSecurityToken Token = _authHelper.GenerateToken(user, RolesList);
            return Token;
        }

        async public  Task<JwtSecurityToken> Login(IdentityUser user, string password)
        {
            _authHelper.IsValidEmail(user.Email);
            if (password == null) { throw new Exception("Password not given"); }
            if (user.Email == null) { throw new Exception("Incorrect email"); }
            if (user.Email != "email@gmail.com") { throw new Exception("User does not exist"); }
            if(password != "bijen") { throw new Exception("Incorrect Password"); }
            IList <String> roleslist = new List<String>
            {
                "DOCTOR"
            };
            JwtSecurityToken token = _authHelper.GenerateToken(user, roleslist);
                return token;
        }

        public Task<IdentityUser> GetCurrentuser(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> Update(UserInformation oldUserInformation, IdentityUser user)
        {
            throw new NotImplementedException();
        }

        async public Task<IdentityUser> GetUserByEmail(string email)
        {
            IdentityUser User = new IdentityUser { Email = "email@gmail.com" };
            return User;
        }
    }
}
