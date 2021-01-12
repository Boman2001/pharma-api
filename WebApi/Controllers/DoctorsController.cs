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
using Core.DomainServices;
using Core.DomainServices.Helpers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DoctorsController : ControllerBase
    {
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IIdentityRepository _identityRepository;


        public DoctorsController(IRepository<UserInformation> userInformationRepository, IIdentityRepository identityRepository)
        {
            _userInformationRepository = userInformationRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInformation>>> GetDoctorsAsync()
        {
            try
            {
                var userinformations = _userInformationRepository.GetAll();
                foreach (var var in userinformations)
                {
                    if (var.UserId != null)
                    {
                        var.User = await _identityRepository.GetUserById(var.UserId.ToString());
                    }
                }

                return Ok(userinformations);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<UserInformation>> GetDoctorsFilter([FromQuery] UserInformation userInformation)
        {
            try
            {
                var filter = _userInformationRepository.Get(dr =>
                        dr.User.UserName.Contains(userInformation.User.UserName));

                return Ok(filter);
            }
            catch (Exception ex)
            {   
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInformation>> GetDoctor(int id)
        {
            var result = _userInformationRepository.Get(s =>  s.Id == id).SingleOrDefault();
            var user = await _identityRepository.GetUserById(result.UserId.ToString());
            result.User = user;
            
            return result == null  ? StatusCode(204) : (ActionResult<UserInformation>) Ok(result);
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
            catch(Exception e)
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
                userDto.User = user;
                userDto.UserId = Guid.Parse(user.Id);
                await _userInformationRepository.Add(userDto);
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(result),
                    User = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}
