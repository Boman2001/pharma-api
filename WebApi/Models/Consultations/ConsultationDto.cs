namespace WebApi.Models.Consultations
{
    using Core.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Users;

    public class ConsultationDto: BaseConsultationDto
    {
        public int Id { get; set; }

        public UserDto Doctor { get; set; }

        public Patient Patient { get; set; }

        public List<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
        public List<Episode> Episodes { get; set; }
        public List<Intolerance> Intolerances { get; set; }
        public List<PhysicalExamination> PhysicalExaminations { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<UserJournal> UserJournals { get; set; }
    }
}