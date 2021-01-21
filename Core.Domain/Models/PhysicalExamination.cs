using System;
using System.Text.Json.Serialization;

namespace Core.Domain.Models
{
    public class PhysicalExamination : BaseEntity
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public int ConsultationId { get; set; }
        [JsonIgnore]    public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        [JsonIgnore]    public Patient Patient { get; set; }

        public int ExaminationTypeId { get; set; }
        [JsonIgnore]     public ExaminationType ExaminationType { get; set; }
    }
}