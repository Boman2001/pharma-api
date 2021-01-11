using System;
using System.Collections.Generic;

namespace Core.Domain
{
    public class Patient : BaseEntitySoftDeletes
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
        
        public List<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
        public List<Consultation> Consultations { get; set; }
        public List<Episode> Episodes { get; set; }
        public List<Intolerance> Intolerances { get; set; }
        public List<PhysicalExamination> PhysicalExaminations { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<UserJournal> UserJournals { get; set; }
    }
}