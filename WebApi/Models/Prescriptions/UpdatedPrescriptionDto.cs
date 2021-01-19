using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class UpdatedPrescriptionDto : BasePrescriptionDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }
    }
}