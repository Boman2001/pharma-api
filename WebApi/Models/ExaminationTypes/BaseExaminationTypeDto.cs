using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.ExaminationTypes
{
    public class BaseExaminationTypeDto
    {
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Eenheid is verplicht.")]
        public string Unit { get; set; }
    }
}