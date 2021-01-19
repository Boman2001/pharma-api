using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;

namespace WebApi.Models.AdditionalExaminationResults
{
    public class CreateAdditionalExaminationResultDto : BaseAdditionalExaminationResultDto
    {
        [Required(ErrorMessage = "Consult Id is verplicht.")]
        public int ConsultationId { get; set; }

        [Required(ErrorMessage = "Patiënt Id is verplicht.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Aanvullend onderzoek type Id is verplicht.")]
        public int AdditionalExaminationTypeId { get; set; }
    }
}