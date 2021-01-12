using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;

namespace Core.Domain.DataTransferObject
{
    public class UserDto : UserInformation
    {
        public string id;

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
