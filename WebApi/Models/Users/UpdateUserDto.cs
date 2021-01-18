namespace WebApi.Models.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class UpdateUserDto : BaseUserDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}