using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Models.Authentication
{
    public class TwoFactorDto
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "E-mailadres is geen geldig e-mailadres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Twee Factor is Gefaald")]
        public string Code { get; set; }
    }
}