using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Helpers;
using Core.DomainServices.Repositories;
using Infrastructure;
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
        private readonly MultiFactorAuthenticationHelper _multiFactorAuthenticationHelper;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthController(IIdentityRepository identityRepository,
            IRepository<UserInformation> userInformationRepository,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDbContext)
        {
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
            _multiFactorAuthenticationHelper = new MultiFactorAuthenticationHelper(userManager, identityRepository);
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("login/twofactor")]
        public async Task<IActionResult> TwoFactor([FromBody] TwoFactorDto login)
        {
            var user = await _identityRepository.GetUserByEmail(login.Email);
            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == user.Id).FirstOrDefault();
            SecurityToken securityToken;

            if (user == null || userInformation == null) return NotFound();
            try
            {
                // Strip spaces and hypens
                var verificationCode = login.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
                securityToken = await _multiFactorAuthenticationHelper.ValidateTwoFactor(user, verificationCode);

                if (!user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = true;
                    // @TODO User Update?
                    // (Can't really be done right here right now because the user repo requires a password.)
                    await this._applicationDbContext.SaveChangesAsync();   
                }

                var userDto = new UserDto
                {
                    Id = Guid.Parse(user.Id),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Name = userInformation.Name,
                    Dob = userInformation.Dob,
                    Gender = userInformation.Gender,
                    City = userInformation.City,
                    Street = userInformation.Street,
                    HouseNumber = userInformation.HouseNumber,
                    HouseNumberAddon = userInformation.HouseNumberAddon,
                    PostalCode = userInformation.PostalCode,
                    Country = userInformation.Country
                };

                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(securityToken), User = userDto});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
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
                var user = await _identityRepository.GetUserByEmail(identityUser.Email);
                
                return Ok(new
                {
                    TwoFactorUrl = user.TwoFactorEnabled ? null : await _multiFactorAuthenticationHelper.LoadSharedKeyAndQrCodeUriAsync(user),
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}