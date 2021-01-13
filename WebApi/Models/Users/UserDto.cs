using System;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class UserDto : BaseUserDto
    {
        public Guid Id { get; set; }
    }
}