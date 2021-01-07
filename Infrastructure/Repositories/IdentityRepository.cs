using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthHelper _authHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        public IdentityRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authHelper = new AuthHelper(configuration);
        }

        async public Task<JwtSecurityToken> Register(IdentityUser user, string password)
        {
            _authHelper.IsValidEmail(user.Email);
            IdentityResult Result = await _userManager.CreateAsync(user, password);
            ErrorHandling(Result);
            await _userManager.AddToRoleAsync(user, "Doctor");
            IList<string> RoleList = await _userManager.GetRolesAsync(user);
            JwtSecurityToken Token = _authHelper.GenerateToken(user, RoleList);
            return Token;
        }

        public async Task<JwtSecurityToken> Login(IdentityUser user, string password)
        {
            _authHelper.IsValidEmail(user.Email);
            IdentityUser Result = await _userManager.FindByEmailAsync(user.Email);
            SignInResult LoginResult = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            IList<string> RoleList = await _userManager.GetRolesAsync(Result);
            if (LoginResult.Succeeded) {
                JwtSecurityToken Token = _authHelper.GenerateToken(user, RoleList);
                return Token;
            } else {
               throw new Exception("Password Incorrect");
            }
        }

        public static void ErrorHandling(IdentityResult result)
        {
            List<Exception> Exceptions = new List<Exception>();
            foreach (IdentityError error in result.Errors)
            {
                Exceptions.Add(new Exception(error.Description));
            }
            if (Exceptions.Count >= 1)
            {
                throw new AggregateException("Encountered errors while trying to do something.", Exceptions);
            }
        }

        public Task<IdentityUser> GetCurrentuser(ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);
            var result = _userManager.FindByEmailAsync(userEmail);
            return result;
        }
    }
}
