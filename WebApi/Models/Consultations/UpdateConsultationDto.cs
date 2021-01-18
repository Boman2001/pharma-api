namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateConsultationDto : BaseConsultationDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }
    }
}