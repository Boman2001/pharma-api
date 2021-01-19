#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Prescriptions
{
    public class BasePrescriptionDto
    {
        [Required(ErrorMessage = "Omschrijving is verplicht.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start datum is verplicht.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Eind datum is verplicht.")]
        public DateTime? EndDate { get; set; }
    }
}