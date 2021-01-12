using Core.Domain;
using Core.Domain.DataTransferObject;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices;
using Microsoft.AspNetCore.Authorization;
using WebApi.Mappings;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly IMapper _mapper;

        public UsersController(IMapper autoMapper, IRepository<UserInformation> userInformationRepository,
            IIdentityRepository identityRepository)
        {
            _userInformationRepository = userInformationRepository;
            _identityRepository = identityRepository;
            _mapper = autoMapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInformation>>> GetDoctorsAsync()
        {
            try
            {
                List<UserInformationDto> userInformationDtos = new List<UserInformationDto>();
                var userinformations = _userInformationRepository.Get();
                foreach (var var in userinformations)
                {
                    var p = _mapper.Map<UserInformationDto>(var);
                    if (var.UserId != Guid.Empty)
                    {
                        var.User = await _identityRepository.GetUserById(var.UserId.ToString());
                        p.Email = var.User.Email.ToString();
                    }


                    p.StringId = var.Id.ToString();
                    userInformationDtos.Add(p);
                }

                return Ok(userInformationDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        //[HttpGet("filter")]
        //public ActionResult<IEnumerable<UserInformation>> GetDoctorsFilter([FromQuery] UserInformation userInformation)
        //{
        //    try
        //    {
        //        var filter = _userInformationRepository.Get(dr =>
        //                dr.User.UserName.Contains(userInformation.User.UserName));

        //        return Ok(filter);
        //    }
        //    catch (Exception ex)
        //    {   
        //        return BadRequest(new {message = ex.Message});
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<UserInformationDto>> GetDoctor(int id)
        {
            var result = _userInformationRepository.Get(s => s.Id == id).SingleOrDefault();

            if (result != null)
            {
                var user = await _identityRepository.GetUserById(result.UserId.ToString());
                var p = _mapper.Map<UserInformationDto>(result);
                p.StringId = result.Id.ToString();
                p.Email = user.Email.ToString();
                return result == null ? StatusCode(204) : (ActionResult<UserInformationDto>) Ok(p);
            }

            return StatusCode(204);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] UserDto userDto)
        {
            userDto.UserId = new Guid();
            userDto.Id = id;
            var identityUser = new IdentityUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                PasswordHash = userDto.Password
            };
            try
            {
                var userInformation = await _userInformationRepository.Get(id);
                await _identityRepository.Update(identityUser, userInformation);
                _userInformationRepository.Detach(userInformation);
                userDto.UserId = userInformation.UserId;
                await _userInformationRepository.Update(userDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var userInformation = _userInformationRepository.Get(id).Result;
                var user = _identityRepository.GetUserById(userInformation.UserId.ToString()).Result;
                await _identityRepository.DeleteUser(user);
                await _userInformationRepository.Delete(userInformation);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e});
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post([FromForm] UserDto userDto)
        {
            userDto.UserId = new Guid();
            var identityUser = new IdentityUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                PasswordHash = userDto.Password,
            };

            try
            {
                var result = await _identityRepository.Register(identityUser, identityUser.PasswordHash);
                var user = await _identityRepository.GetUserByEmail(identityUser.Email);

                userDto.UserId = Guid.Parse(user.Id);
                var a = await _userInformationRepository.Add(userDto);


                var p = _mapper.Map<UserInformationDto>(a);
                p.StringId = user.Id.ToString();
                p.Email = user.Email;

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(result),
                    User = p
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}