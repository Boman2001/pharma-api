using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Helpers;
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
        private readonly IMapper _mapper;
        private readonly MultiFactorAuthenticationHelper _multiFactorAuthenticationHelper;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public AuthController(IIdentityRepository identityRepository,
            IRepository<UserInformation> userInformationRepository, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
            _mapper = mapper;
            _multiFactorAuthenticationHelper = new MultiFactorAuthenticationHelper(userManager, identityRepository);
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
            var identityUser = _mapper.Map<LoginDto, IdentityUser>(login);

            identityUser.PasswordHash = login.Password;
            identityUser.UserName = identityUser.Email;

            SecurityToken securityToken;

            try
            {
                securityToken = await _identityRepository.Login(identityUser, identityUser.PasswordHash);
                var user = await _identityRepository.GetUserByEmail(identityUser.Email);
                return Ok(new
                {
                    TwoFactorUrl = await _multiFactorAuthenticationHelper.LoadSharedKeyAndQrCodeUriAsync(user),
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}