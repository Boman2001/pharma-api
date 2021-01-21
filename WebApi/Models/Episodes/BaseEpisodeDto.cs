#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Episodes
{
    public class BaseEpisodeDto
    {
        [Required(ErrorMessage = "Omschrijving is verplicht.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Priority is verplicht.")]
        public int? Priority { get; set; }

        [Required(ErrorMessage = "Start datum is verplicht.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int? ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt is verplicht.")]
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "ICPC Code is verplicht.")]
        public int? IcpcCodeId { get; set; }
    }
}