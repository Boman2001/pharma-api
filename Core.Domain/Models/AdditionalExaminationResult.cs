using System;
using System.Text.Json.Serialization;

namespace Core.Domain.Models
{
    public class AdditionalExaminationResult : BaseEntity
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public int ConsultationId { get; set; }
        [JsonIgnore] public Consultation Consultation { get; set; }

        public int PatientId { get; set; }

        [JsonIgnore] public Patient Patient { get; set; }

        public int AdditionalExaminationTypeId { get; set; }

        [JsonIgnore] public AdditionalExaminationType AdditionalExaminationType { get; set; }
    }
}