using System;

namespace WebApi.Models.Users
{
    public class UserDto : BaseUserDto
    {
        public Guid Id { get; set; }
    }
}