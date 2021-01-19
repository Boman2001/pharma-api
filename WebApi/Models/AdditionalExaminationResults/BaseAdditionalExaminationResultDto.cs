#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;

namespace WebApi.Models.AdditionalExaminationResults
{
    public class BaseAdditionalExaminationResultDto
    {
        [Required(ErrorMessage = "Waarde is verplicht.")]
        public string? Value { get; set; }

        [Required(ErrorMessage = "Datum is verplicht.")]
        public DateTime? Date { get; set; }
    }
}