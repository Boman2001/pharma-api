using System;
using Core.Domain.Models;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ConsultationDto Consultation { get; set; }
        public PatientDto Patient { get; set; }
    }
}