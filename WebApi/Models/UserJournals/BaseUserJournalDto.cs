#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.UserJournals
{
    public class BaseUserJournalDto
    {
        [Required(ErrorMessage = "Omschrijving is verplicht.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Type is verplicht.")]
        public int? Property { get; set; }
        
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int? ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt is verplicht.")]
        public int? PatientId { get; set; }
    }
}