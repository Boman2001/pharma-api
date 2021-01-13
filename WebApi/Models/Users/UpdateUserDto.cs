using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class UpdateUserDto : BaseUserDto
    {
        public string Password { get; set; }
    }
}