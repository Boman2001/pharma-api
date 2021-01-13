using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.DomainServices.Helpers;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthHelper _authHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SecurityDbContext _dbContext;

        public IdentityRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, SecurityDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _authHelper = new AuthHelper(configuration);
        }

        public async Task<JwtSecurityToken> Register(IdentityUser user, string password)
        {
            var findByEmailAsync = await _userManager.FindByEmailAsync(user.Email);
            
            if (findByEmailAsync != null)
            {
                throw new Exception("E-mailadres is al in gebruik.");
            }

            var result = await _userManager.CreateAsync(user, password);
            ErrorHandling(result);
            
            await _userManager.AddToRoleAsync(user, "Doctor");
            var roleList = await _userManager.GetRolesAsync(user);
            var token = _authHelper.GenerateToken(user, roleList);
            
            return token;
        }

        public async Task<JwtSecurityToken> Login(IdentityUser user, string password)
        {
            if (AuthHelper.IsValidEmail(user.Email) == false)
            {
                throw new Exception("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
            }

            var result = await _userManager.FindByEmailAsync(user.Email);

            if (result == null)
            {
                throw new Exception("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
            }

            var loginResult = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            var roleList = await _userManager.GetRolesAsync(result);

            if (loginResult.Succeeded)
            {
                var token = _authHelper.GenerateToken(user, roleList);
                return token;
            }

            throw new Exception("Deze combinatie van e-mailadres en wachtwoord is incorrect.");
        }

        public async Task<IdentityResult> DeleteUser(IdentityUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public static void ErrorHandling(IdentityResult result)
        {
            var exceptions = result.Errors.Select(error => new Exception(error.Description)).ToList();

            if (exceptions.Count >= 1)
            {
                throw new AggregateException(exceptions);
            }
        }

        public Task<IdentityUser> GetCurrentUser(ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);
            var result = _userManager.FindByEmailAsync(userEmail);
            return result;
        }

        public async Task<IdentityResult> Update(IdentityUser user)
        {
            var result = await _userManager.FindByIdAsync(user.Id);

            if (result == null)
            {
                throw new Exception("Gebruiker bestaat niet.");
            }

            var findByEmailAsync = await _userManager.FindByEmailAsync(user.Email);
            if (findByEmailAsync != null && findByEmailAsync.Email == result.Email)
            {
                throw new Exception("E-mailadres is al in gebruik.");
            }

            result.Email = user.Email;
            result.UserName = user.UserName;
            result.PhoneNumber = user.PhoneNumber;
            result.PasswordHash = AuthHelper.HashPassword(user.PasswordHash);
            
            return await _userManager.UpdateAsync(result);
        }

        public async Task<IdentityUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public void Detach(IEnumerable<IdentityUser> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
        }
    }
}