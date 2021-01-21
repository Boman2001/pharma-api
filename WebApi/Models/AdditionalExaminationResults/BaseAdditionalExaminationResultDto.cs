#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.AdditionalExaminationResults
{
    public class BaseAdditionalExaminationResultDto
    {
        [Required(ErrorMessage = "Waarde is verplicht.")]
        public string? Value { get; set; }

        [Required(ErrorMessage = "Datum is verplicht.")]
        public DateTime? Date { get; set; }
        
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int? ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "Aanvullend onderzoek type Id is verplicht.")]
        public int? AdditionalExaminationTypeId { get; set; }
    }
}