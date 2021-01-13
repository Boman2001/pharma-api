﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.DomainServices.Repositories
{
    public interface IIdentityRepository
    {
        Task<JwtSecurityToken> Login(IdentityUser user, string password);

        Task<IdentityResult> Update(IdentityUser user, string password);

        Task<IdentityResult> Delete(IdentityUser user);

        Task<IdentityUser> GetCurrentUser(ClaimsPrincipal user);

        Task<IdentityUser> GetUserById(string id);

        Task<IdentityUser> GetUserByEmail(string email);
    }
}