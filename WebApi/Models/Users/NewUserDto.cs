using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Users
{
    using System.Diagnostics.CodeAnalysis;

    public class NewUserDto : BaseUserDto
    {
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }
    }
}