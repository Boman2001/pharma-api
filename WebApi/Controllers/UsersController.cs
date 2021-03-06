﻿using System;
using System.Collections.Generic;
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
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(
            IMapper autoMapper,
            IRepository<UserInformation> userInformationRepository,
            IIdentityRepository identityRepository,
            UserManager<IdentityUser> userManager)
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

            if (result == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<IdentityUser, UserDto>(result);
            var userInformation = _userInformationRepository.Get(u => u.UserId == user.Id)
                .FirstOrDefault();

            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);
            _mapper.Map(userInformationDto, user);

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserDto>> Post([FromBody] CreateUserDto createUserDto)
        {
            var identityUser = new IdentityUser
            {
                Email = createUserDto.Email,
                UserName = createUserDto.Email,
                PhoneNumber = createUserDto.PhoneNumber,
                PasswordHash = createUserDto.Password
            };

            var checkEmail = await _identityRepository.GetUserByEmail(identityUser.Email);

            if (checkEmail != null)
            {
                return BadRequest("E-mailadres is al in gebruik.");
            }

            try
            {
                identityUser = await _identityRepository.Register(identityUser, identityUser.PasswordHash);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }

            var userInformation = new UserInformation
            {
                Name = createUserDto.Name,
                Dob = createUserDto.Dob,
                Gender = createUserDto.Gender,
                City = createUserDto.City,
                Street = createUserDto.Street,
                HouseNumber = createUserDto.HouseNumber,
                HouseNumberAddon = createUserDto.HouseNumberAddon,
                PostalCode = createUserDto.PostalCode,
                Country = createUserDto.Country,
                UserId = Guid.Parse(identityUser.Id)
            };

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            try
            {
                await _userInformationRepository.Add(userInformation, currentUser);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });

                //TODO rollback identityuser
            }

            var user = new UserDto
            {
                Id = Guid.Parse(identityUser.Id),
                Email = identityUser.Email,
                PhoneNumber = identityUser.PhoneNumber,
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

            return Created(nameof(Post), user);
        }

        [Authorize(Roles = "Admin")]
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
            user.UserName = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;

            try
            {
                await _identityRepository.Update(user, updateUserDto.Password);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }

            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == id).FirstOrDefault();

            if (userInformation == null)
            {
                return NotFound();
            }

            userInformation.Name = updateUserDto.Name;
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
                var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
                var currentUser = await _identityRepository.GetUserById(userId);

                userInformation = await _userInformationRepository.Update(userInformation, currentUser);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }

            var userDto = _mapper.Map<IdentityUser, UserDto>(user);

            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);
            _mapper.Map(userInformationDto, userDto);

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _identityRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            await _identityRepository.Delete(user);

            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == user.Id)
                .FirstOrDefault();

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _userInformationRepository.Delete(userInformation, currentUser);

            return NoContent();
        }
    }
}