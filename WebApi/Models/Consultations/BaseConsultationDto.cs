#nullable enable
namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BaseConsultationDto
    {
        [Required(ErrorMessage = "Datum is verplicht.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Opmerkingen zijn verplicht.")]
        public string? Comments { get; set; }

        [Required(ErrorMessage = "Arts Id is verplicht.")]
        public Guid? DoctorId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int? PatientId { get; set; }
        
        public bool? Completed { get; set; }
    }
}