using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Users;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(IMapper autoMapper, IRepository<UserInformation> userInformationRepository,
            IIdentityRepository identityRepository, UserManager<IdentityUser> userManager)
        {
            _userInformationRepository = userInformationRepository;
            _identityRepository = identityRepository;
            _userManager = userManager;
            _mapper = autoMapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<IdentityUser>> Get()
        {
            var results = _userManager.Users;

            var users = _mapper.Map<IEnumerable<IdentityUser>, IEnumerable<UserDto>>(results);

            var userDtos = new List<UserDto>();

            users.ToList().ForEach(user =>
            {
                var userInformation = _userInformationRepository.Get(u => u.UserId == user.Id)
                    .FirstOrDefault();

                var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);
                _mapper.Map(userInformationDto, user);

                userDtos.Add(user);
            });

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserInformationDto>> Get(string id)
        {
            var result = await _identityRepository.GetUserById(id);

            if (result == null) return NotFound();

            var user = _mapper.Map<IdentityUser, UserDto>(result);
            var userInformation = _userInformationRepository.Get(u => u.UserId == user.Id)
                .FirstOrDefault();
            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);
            _mapper.Map(userInformationDto, user);

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserDto>> Post([FromBody] NewUserDto newUserDto)
        {
            var identityUser = new IdentityUser
            {
                Email = newUserDto.Email,
                UserName = newUserDto.Email,
                PhoneNumber = newUserDto.PhoneNumber,
                PasswordHash = newUserDto.Password
            };

            JwtSecurityToken result;

            try
            {
                result = await _identityRepository.Register(identityUser, identityUser.PasswordHash);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }

            identityUser = await _identityRepository.GetUserByEmail(identityUser.Email);

            var userInformation = new UserInformation
            {
                Name = newUserDto.Name,
                Bsn = newUserDto.Bsn,
                Dob = newUserDto.Dob,
                Gender = newUserDto.Gender,
                City = newUserDto.City,
                Street = newUserDto.Street,
                HouseNumber = newUserDto.HouseNumber,
                HouseNumberAddon = newUserDto.HouseNumberAddon,
                PostalCode = newUserDto.PostalCode,
                Country = newUserDto.Country,
                UserId = Guid.Parse(identityUser.Id)
            };

            await _userInformationRepository.Add(userInformation);

            var user = new UserDto
            {
                Id = Guid.Parse(identityUser.Id),
                Email = identityUser.Email,
                PhoneNumber = identityUser.PhoneNumber,
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
                Token = new JwtSecurityTokenHandler().WriteToken(result),
                user
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _identityRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = updateUserDto.Email;
            user.Email = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;

            try
            {
                await _identityRepository.Update(user, updateUserDto.Password);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }

            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == id).FirstOrDefault();

            if (userInformation == null)
            {
                return NotFound();
            }

            userInformation.Name = updateUserDto.Name;
            userInformation.Bsn = updateUserDto.Bsn;
            userInformation.Dob = updateUserDto.Dob;
            userInformation.Gender = updateUserDto.Gender;
            userInformation.City = updateUserDto.City;
            userInformation.Street = updateUserDto.Street;
            userInformation.HouseNumber = updateUserDto.HouseNumber;
            userInformation.HouseNumberAddon = updateUserDto.HouseNumberAddon;
            userInformation.PostalCode = updateUserDto.PostalCode;
            userInformation.Country = updateUserDto.Country;

            try
            {
                userInformation = await _userInformationRepository.Update(userInformation);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }

            var userDto = _mapper.Map<IdentityUser, UserDto>(user);

            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);
            _mapper.Map(userInformationDto, userDto);

            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _identityRepository.GetUserById(id);

            if (user == null) return NotFound();

            await _identityRepository.Delete(user);

            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == user.Id)
                .FirstOrDefault();
            await _userInformationRepository.Delete(userInformation);

            return NoContent();
        }
    }
}