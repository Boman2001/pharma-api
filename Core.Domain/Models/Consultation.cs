using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Models
{
    public class Consultation : BaseEntitySoftDeletes
    {
        public DateTime Date { get; set; }
        public string Comments { get; set; }

        public int DoctorId { get; set; }
        [NotMapped] public IdentityUser Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public List<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
        public List<Episode> Episodes { get; set; }
        public List<Intolerance> Intolerances { get; set; }
        public List<PhysicalExamination> PhysicalExaminations { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<UserJournal> UserJournals { get; set; }
    }
}