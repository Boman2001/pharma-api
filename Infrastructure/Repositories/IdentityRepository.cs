using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.DomainServices.Helpers;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly AuthHelper _authHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityRepository(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authHelper = new AuthHelper(configuration);
        }

        public async Task<IdentityUser> Register(IdentityUser user, string password)
        {
            var findByEmailAsync = await _userManager.FindByEmailAsync(user.Email);

            if (findByEmailAsync != null)
            {
                throw new Exception("E-mailadres is al in gebruik.");
            }

            var result = await _userManager.CreateAsync(user, password);
            ErrorHandling(result);

            await _userManager.AddToRoleAsync(user, "Doctor");
            
            return user;
        }
        
        public async Task<JwtSecurityToken> Login(IdentityUser user, string password)
        {
            var result = await _userManager.FindByEmailAsync(user.Email);

            if (result == null)
            {
                throw new ArgumentException("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
            }

            var loginResult = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (!loginResult.Succeeded)
            {
                throw new ArgumentException("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
            }

            var roles = await _userManager.GetRolesAsync(result);
            var token = _authHelper.GenerateToken(result, roles);
            return token;
        }

        public async Task<JwtSecurityToken> GetTokenForTwoFactor(IdentityUser user)
        {
            var result = await _userManager.FindByEmailAsync(user.Email);

            if (result == null)
            {
                throw new Exception("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
            }

            var roles = await _userManager.GetRolesAsync(result);

            var token = _authHelper.GenerateToken(user, roles);
            return token;
        }

        public async Task<IdentityResult> Update(IdentityUser user, string password = null)
        {
            var result = await _userManager.FindByIdAsync(user.Id);

            if (result == null)
            {
                throw new ArgumentException("Gebruiker bestaat niet.");
            }

            var findByEmailAsync = await _userManager.FindByEmailAsync(result.Email);
            if (findByEmailAsync?.Id != result.Id && findByEmailAsync?.Email == result.Email)
            {
                throw new ArgumentException("E-mailadres is al in gebruik.");
            }

            result.Email = user.Email;
            result.UserName = user.UserName;
            result.PhoneNumber = user.PhoneNumber;

            if (password != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, password);

                if (!passwordCheck)
                {
                    throw new ArgumentException("Wachtwoord is ongeldig.");
                }

                result.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
            }

            return await _userManager.UpdateAsync(result);
        }

        public async Task<IdentityResult> Delete(IdentityUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public Task<IdentityUser> GetCurrentUser(ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);
            return _userManager.FindByEmailAsync(userEmail);
        }

        public async Task<IdentityUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private static void ErrorHandling(IdentityResult result)
        {
            var exceptions = result.Errors.Select(error => new Exception(error.Description)).ToList();

            if (exceptions.Count >= 1)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}