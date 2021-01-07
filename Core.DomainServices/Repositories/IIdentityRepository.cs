using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public interface IIdentityRepository
    {
        Task<JwtSecurityToken> Register(IdentityUser user, string password);

        Task<JwtSecurityToken> Login(IdentityUser user, string password);

        Task<IdentityUser> GetCurrentuser(ClaimsPrincipal user);
    }
}
