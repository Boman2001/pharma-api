﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Helpers;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(
            IIdentityRepository identityRepository,
            IRepository<UserInformation> userInformationRepository,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
            _multiFactorAuthenticationHelper = new MultiFactorAuthenticationHelper(userManager, identityRepository, configuration);
            _userManager = userManager;
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
                    await _identityRepository.Update(user, null);
                }

                var rolesList = await _userManager.GetRolesAsync(user);
                var userDto = new RoleDto
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
                    Country = userInformation.Country,
                    Roles = rolesList
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

            try
            {
                await _identityRepository.Login(identityUser, identityUser.PasswordHash);
                var user = await _identityRepository.GetUserByEmail(identityUser.Email);
                
                return Ok(new
                {
                    TwoFactorUrl = user.TwoFactorEnabled ? null : await _multiFactorAuthenticationHelper.LoadSharedKeyAndQrCodeUriAsync(user), user.Email
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