using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;

namespace Core.Domain.DataTransferObject
{
    public class UserInformationDto
    {
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