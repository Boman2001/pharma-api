namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateConsultationDto : BaseConsultationDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Arts Id is verplicht.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }
    }
}