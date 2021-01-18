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
    }
}