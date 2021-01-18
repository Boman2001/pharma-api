using WebApi.Models.Patients;

namespace WebApi.Models.Consultations
{
    using Core.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Users;

    public class ConsultationDto : BaseConsultationDto
    {
        public int Id { get; set; }
        public UserDto Doctor { get; set; }
        public PatientDto Patient { get; set; }
    }
}