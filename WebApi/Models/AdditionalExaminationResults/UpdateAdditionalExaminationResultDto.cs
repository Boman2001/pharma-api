using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;

namespace WebApi.Models.AdditionalExaminationResults
{
    public class UpdateAdditionalExaminationResultDto : BaseAdditionalExaminationResultDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int ConsultationId { get; set; }
        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Aanvullend onderzoek type Id is verplicht.")]
        public int AdditionalExaminationTypeId { get; set; }
    }
}