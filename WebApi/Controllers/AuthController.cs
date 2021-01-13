using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models.Authentication;
using WebApi.Models.Users;

namespace WebApi.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public AuthController(
            IIdentityRepository identityRepository,
            IRepository<UserInformation> userInformationRepository)
        {
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var identityUser = new IdentityUser
            {
                Email = login.Email, UserName = login.Email, PasswordHash = login.Password
            };

            SecurityToken securityToken;

            try
            {
                securityToken = await _identityRepository.Login(identityUser, identityUser.PasswordHash);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            var user = await _identityRepository.GetUserByEmail(identityUser.Email);
            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == user.Id).FirstOrDefault();

            if (user == null || userInformation == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = userInformation.Name,
                Bsn = userInformation.Bsn,
                Dob = userInformation.Dob, 
                Gender = userInformation.Gender,
                City = userInformation.City,
                Street = userInformation.Street,
                HouseNumber = userInformation.HouseNumber,
                HouseNumberAddon = userInformation.HouseNumberAddon,
                PostalCode = userInformation.PostalCode,
                Country = userInformation.Country
            };

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken), User = userDto
            });
        }
    }
}