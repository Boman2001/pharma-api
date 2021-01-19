using System;
using Core.Domain.Models;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class CreatedPrescriptionDto : BasePrescriptionDto
    {
        public int Id { get; set; }
    }
}