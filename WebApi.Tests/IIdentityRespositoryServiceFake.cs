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
    class IdentityRepositoryServiceFake : IIdentityRepository
    {
        private readonly AuthHelper _authHelper;
        public IdentityRepositoryServiceFake(IConfiguration configuration)
        {
            _authHelper = new AuthHelper(configuration);
        }
        async public Task<JwtSecurityToken> Create(User User, string Password)
        {

            if (Password == null) { throw new Exception("Password not given"); }
            if (User.Email == null) { throw new Exception("The specified string is not in the form required for an e-mail address."); }
            _authHelper.IsValidEmail(User.Email);
            IList<String> RolesList = new List<String>
            {
                "DOCTOR"
            };

            JwtSecurityToken Token = _authHelper.GenerateToken(User, RolesList);
            return Token;
        }

        async public  Task<JwtSecurityToken> Login(User User, string Password)
        {
            _authHelper.IsValidEmail(User.Email);
            if (Password == null) { throw new Exception("Password not given"); }
            if (User.Email == null) { throw new Exception("The specified string is not in the form required for an e-mail address."); }
            if (User.Email != "email@gmail.com") { throw new Exception("User doesnt exist"); }
            if(Password != "bijen") { throw new Exception("Wrong password"); }
            IList <String> roleslist = new List<String>
            {
                "DOCTOR"
            };
            JwtSecurityToken token = _authHelper.GenerateToken(User, roleslist);
                return token;
        }


    }
}
