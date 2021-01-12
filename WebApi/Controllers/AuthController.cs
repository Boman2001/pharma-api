using Core.DomainServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain;
using Core.Domain.DataTransferObject;
using Core.Domain.Models;
using Core.DomainServices.Repositories;

namespace WebApi.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IMapper _mapper;

        public AuthController(IMapper autoMapper, IIdentityRepository identityRepository, IRepository<UserInformation> userInformationRepository )
        {
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
            _mapper = autoMapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] IdentityUser model, [FromForm] string password)
        {
            model.PasswordHash = password;
            model.UserName = model.Email;
            try
            {
                var resultJwtSecurityToken = await _identityRepository.Register(model, model.PasswordHash);

                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(resultJwtSecurityToken)});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromForm] IdentityUser model, [FromForm] string password)
        {
            model.PasswordHash = password;
            model.UserName = model.Email;
            try
            {
                var result = await _identityRepository.Login(model, model.PasswordHash);
                var user = await _identityRepository.GetUserByEmail(model.Email);

                var b = _userInformationRepository.Get(d => d.UserId == Guid.Parse(user.Id)).FirstOrDefault();
                
                var p = _mapper.Map<UserInformationDto>(b);
                p.Email = user.Email;

                p.StringId = user.Id.ToString();


                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(result), User = p });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }


        //Example off authorized route
        //Header required:
        //Authorization Bearer [Token]
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var user =  await _IdentityRepository.GetCurrentuser(User);
        //    return Ok(user);
        //}
    }
}
