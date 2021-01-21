using System;

namespace WebApi.Models.Users
{
    public class UpdateUserDto : BaseUserDto
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}