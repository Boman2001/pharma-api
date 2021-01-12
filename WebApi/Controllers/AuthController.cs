using Core.DomainServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;

        public AuthController(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
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

                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(result), User = user});
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
