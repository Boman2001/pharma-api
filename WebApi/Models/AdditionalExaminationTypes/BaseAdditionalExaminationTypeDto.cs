using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Models;

namespace WebApi.Models.AdditionalExaminationTypes
{
    public class BaseAdditionalExaminationTypeDto
    {
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Eenheid is verplicht.")]
        public string Unit { get; set; }
    }
}