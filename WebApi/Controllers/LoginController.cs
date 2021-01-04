using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Core.DomainServices;
using Core.DomainServices.Helper;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IIdentityRepository _IdentityRepository;
        private readonly TokenController _tokenController;
        public LoginController( IIdentityRepository identityRepository, IConfiguration configuration)
        {
            _tokenController = new TokenController(configuration);
            _IdentityRepository = identityRepository;
        }
       

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] User model)
        {
            try
            {
                var Result = await _IdentityRepository.Create(model, model.PasswordHash);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromForm] User model)
        {
            try
            {
                var Result = await _IdentityRepository.login(model, model.PasswordHash);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
