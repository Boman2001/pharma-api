namespace WebApi.Models.Consultations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class CreatedConsultationDto : BaseConsultationDto
    {
        public int Id { get; set; }
    }
}