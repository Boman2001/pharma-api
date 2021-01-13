using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class BaseUserDto
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "E-mailadres is geen geldig e-mailadres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "BSN is verplicht.")]
        public string Bsn { get; set; }

        [Required(ErrorMessage = "Geboortedatum is verplicht.")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Geslacht is verplicht.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Woonplaats is verplicht.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Straat is verplicht.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Huisnummer is verplicht.")]
        public string HouseNumber { get; set; }

        [Required(ErrorMessage = "Toevoeging is verplicht.")]
        public string HouseNumberAddon { get; set; }

        [Required(ErrorMessage = "Postcode is verplicht.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Land is verplicht.")]
        public string Country { get; set; }
    }
}