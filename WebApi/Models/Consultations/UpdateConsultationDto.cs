namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateConsultationDto : BaseConsultationDto
    {
        public int Id { get; set; }
    }
}