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
        Task<JwtSecurityToken> Create(User User, string Password);

        Task<JwtSecurityToken> Login(User User, string Password);
    }
}
