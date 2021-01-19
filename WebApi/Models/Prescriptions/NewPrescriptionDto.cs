using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class NewPrescriptionDto : BasePrescriptionDto
    {
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }
    }
}