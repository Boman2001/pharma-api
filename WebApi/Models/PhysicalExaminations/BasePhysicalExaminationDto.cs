#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.PhysicalExaminations
{
    public class BasePhysicalExaminationDto
    {
        [Required(ErrorMessage = "Waarde is verplicht.")]
        public string? Value { get; set; }

        [Required(ErrorMessage = "Datum is verplicht.")]
        public DateTime? Date { get; set; }
        
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int? ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "Onderzoek type Id is verplicht.")]
        public int? ExaminationTypeId { get; set; }
    }
}