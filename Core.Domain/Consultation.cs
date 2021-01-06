using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain
{
    public class Consultation : BaseEntity
    {
        public DateTime Date { get; set; }
        
        public int DoctorId { get; set; }
        public IdentityUser Doctor { get; set; }
        
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}