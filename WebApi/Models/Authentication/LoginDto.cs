﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Authentication
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "E-mailadres is geen geldig e-mailadres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }
    }
}