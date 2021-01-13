using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class NewUserDto
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "E-mailadres is geen geldig e-mailadres.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "PhoneNumber is verplicht")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Name is verplicht")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Bsn is verplicht")]
        public string Bsn { get; set; }
        [Required(ErrorMessage = "Dob is verplicht")]
        public DateTime Dob { get; set; }
        [Required(ErrorMessage = "Gender is verplicht")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "City is verplicht")]
        public string City { get; set; }
        [Required(ErrorMessage = "Street is verplicht")]
        public string Street { get; set; }
        [Required(ErrorMessage = "HouseNumber is verplicht")]
        public string HouseNumber { get; set; }
        [Required(ErrorMessage = "HouseNumberAddon is verplicht")]
        public string HouseNumberAddon { get; set; }
        [Required(ErrorMessage = "PostalCode is verplicht")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Country is verplicht")]
        public string Country { get; set; }
    }
}