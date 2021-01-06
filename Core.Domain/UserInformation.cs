﻿using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain
{
    public class UserInformation : BaseEntity
    {
        public string Name { get; set; }
        public string Bsn { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string HouseNumberAddon { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public int UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}