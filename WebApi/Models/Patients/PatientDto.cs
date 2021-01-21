namespace WebApi.Models.Patients
{
    using Core.Domain.Enums;
    using System;

    public class PatientDto : BasePatientDto
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}