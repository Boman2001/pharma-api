﻿using Core.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public interface IIdentityRepository
    {
        Task<JwtSecurityToken> Create(IdentityUser User, string Password);

        Task<JwtSecurityToken> Login(IdentityUser User, string Password);
    }
}
