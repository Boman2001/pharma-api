using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class NewUserDto : BaseUserDto
    {
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }
    }
}