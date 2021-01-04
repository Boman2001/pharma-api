using Core.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.DomainServices.Helper
{
    public class TokenController
    {
        private readonly IConfiguration _configuration;
        public TokenController( IConfiguration configuration){
            _configuration = configuration;
         }

        public JwtSecurityToken GenerateToken(User User, IList<String> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        
            return new JwtSecurityToken(
              issuer: _configuration["JWT:ValidIssuer"],
              audience: _configuration["JWT:ValidAudience"],
              expires: DateTime.Now.AddHours(3),
              signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)


              ); 
        }


    }
}
