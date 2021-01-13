using System;
using Core.Domain.Enums;

namespace WebApi.Models.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Bsn { get; set; }
        public DateTime Dob { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string HouseNumberAddon { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}