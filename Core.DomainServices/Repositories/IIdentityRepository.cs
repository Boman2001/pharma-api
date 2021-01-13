using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.DomainServices.Repositories
{
    public interface IIdentityRepository
    {
        Task<JwtSecurityToken> Register(IdentityUser user, string password);

        Task<JwtSecurityToken> Login(IdentityUser user, string password);

        Task<IdentityResult> Update(IdentityUser user, string password);

        Task<IdentityUser> GetCurrentUser(ClaimsPrincipal user);

        Task<IdentityUser> GetUserByEmail(string email);

        void Detach(IEnumerable<IdentityUser> entities);

        Task<IdentityResult> DeleteUser(IdentityUser user);

        Task<IdentityUser> GetUserById(string id);
    }
}