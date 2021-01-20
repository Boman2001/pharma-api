namespace WebApi.Models.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateUserDto : BaseUserDto
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}