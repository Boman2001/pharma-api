using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public interface IIdentityRepository
    {
        Task<JwtSecurityToken> Register(IdentityUser User, string Password);

        Task<JwtSecurityToken> Login(IdentityUser User, string Password);
    }
}
