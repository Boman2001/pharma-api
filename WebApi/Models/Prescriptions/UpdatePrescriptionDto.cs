using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class UpdatePrescriptionDto : BasePrescriptionDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }
    }
}