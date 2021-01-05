﻿using System;
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
using Microsoft.AspNetCore.Identity;

namespace WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IIdentityRepository _IdentityRepository;

        public LoginController(IIdentityRepository identityRepository)
        {
            _IdentityRepository = identityRepository;
        }

        


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] User Model, [FromForm] String Password)
        {
            Model.PasswordHash = Password;
            Model.UserName = Model.Email;
            try
            {
                var Result = await _IdentityRepository.Create(Model, Model.PasswordHash);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromForm] User Model, [FromForm] String Password)
        {
            Model.PasswordHash = Password;
            Model.UserName = Model.Email;
            try
            {
                var Result = await _IdentityRepository.Login(Model, Model.PasswordHash);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(Result) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //Example off authorized route
        //Header required:
        //Authorization Bearer [Token]
        [Authorize]
        [HttpGet("get")]
        public string Get()
        {
            return "value";
        }

    }
}
