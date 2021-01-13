﻿using AutoMapper;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Models.Authentication;
using WebApi.Models.Users;

namespace WebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityUser, UserDto>();
            CreateMap<UserInformation, UserDto>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<UserInformationDto, UserDto>();
            CreateMap<UserDto, UserInformationDto>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserDto, UserDto>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<LoginDto, IdentityUser>();
        }
    }
}