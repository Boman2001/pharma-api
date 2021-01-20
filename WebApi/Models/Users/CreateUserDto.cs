using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Users
{
    public class CreateUserDto : BaseUserDto
    {
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }
    }
}