using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Core.Domain;
using Core.DomainServices.Helper;
using Core.DomainServices;
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

        async public Task<JwtSecurityToken> Create(User User, string Password)
        {
            _authHelper.IsValidEmail(User.Email);
            var Result = await _userManager.CreateAsync(User, Password);
            ErrorHandling(Result);
            await _userManager.AddToRoleAsync(User, "Doctor");
            IList<String> RoleList = await _userManager.GetRolesAsync(User);
            JwtSecurityToken Token = _authHelper.GenerateToken(User, RoleList);
            return Token;
        }

        public async Task<JwtSecurityToken> Login(User User, string Password)
        {
            _authHelper.IsValidEmail(User.Email);
            var Result = await _userManager.FindByEmailAsync(User.Email);
            SignInResult LoginResult = await _signInManager.PasswordSignInAsync(User.UserName, Password, false, false);
            IList<String> RoleList = await _userManager.GetRolesAsync(Result);
            if (LoginResult.Succeeded) {
                JwtSecurityToken Token = _authHelper.GenerateToken(User, RoleList);
                return Token;
            } else {
               throw new Exception("Password Incorrect");
            }
        }

        public static void ErrorHandling(IdentityResult Result)
        {
            List<Exception> Exceptions = new List<Exception>();
            foreach (IdentityError error in Result.Errors)
            {
                Exceptions.Add(new Exception(error.Description));
            }
            if (Exceptions.Count >= 1)
            {
                throw new AggregateException("Encountered errors while trying to do something.", Exceptions);
            }
        }
    }
}
