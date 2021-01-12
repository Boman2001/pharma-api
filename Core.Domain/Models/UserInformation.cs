using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Models
{
    public class UserInformation : BaseEntity
    {
        public string Name { get; set; }
        public string Bsn { get; set; }
        public DateTime Dob { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string HouseNumberAddon { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public Guid UserId { get; set; }
        [NotMapped]
        public IdentityUser User { get; set; }
    }
}
