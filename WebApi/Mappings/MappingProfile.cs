using Core.Domain.DataTransferObject;
using AutoMapper;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Models.Users;

namespace WebApi.Mappings
{
    public class MappingProfile :Profile{
        public MappingProfile()
        {
            CreateMap<IdentityUser, UserDto>();
            CreateMap<UserInformation, UserDto>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<UserInformationDto, UserDto>();
            CreateMap<UserDto, UserInformationDto>();
            CreateMap<NewUserDto, UserInformation>();
            CreateMap<NewUserDto, UserDto>();
            CreateMap<UpdateUserDto, UserInformation>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<LoginDto, IdentityUser>();
        }
    }
}
