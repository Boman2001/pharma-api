using System.ComponentModel.DataAnnotations;

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