namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NewConsultationDto : BaseConsultationDto
    {
        [Required(ErrorMessage = "Arts Id is verplicht.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }
    }
}