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
        private readonly IIdentityRepository _IdentityRepository;

        public AuthController(IIdentityRepository identityRepository)
        {
            _IdentityRepository = identityRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] IdentityUser Model, [FromForm] string Password)
        {
            Model.PasswordHash = Password;
            Model.UserName = Model.Email;
            try
            {
                JwtSecurityToken Result = await _IdentityRepository.Register(Model, Model.PasswordHash);

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromForm] IdentityUser Model, [FromForm] string Password)
        {
            Model.PasswordHash = Password;
            Model.UserName = Model.Email;
            try
            {
                JwtSecurityToken Result = await _IdentityRepository.Login(Model, Model.PasswordHash);
                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //Example off authorized route
        //Header required:
        //Authorization Bearer [Token]
        //[Authorize]
        //[HttpGet("get")]
        //public string Get()
        //{
        //    return "value";
        //}

    }
}
