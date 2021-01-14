using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.DomainServices.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _configuration;

        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken GenerateToken(IdentityUser user, IList<string> roles)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, user.Id),
                new(ClaimTypes.Email, user.Email), 
            };

            if (roles != null && roles.Count > 0) claims.Add(new Claim(ClaimTypes.Role, roles[0]));

            return new JwtSecurityToken(
                _configuration["JWT:ValidIssuer"],
                _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}