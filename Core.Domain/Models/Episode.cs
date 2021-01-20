using System;
using System.Text.Json.Serialization;

namespace Core.Domain.Models
{
    public class Episode : BaseEntity
    {
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ConsultationId { get; set; }
        [JsonIgnore] public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        [JsonIgnore] public Patient Patient { get; set; }

        public int IcpcCodeId { get; set; }
        [JsonIgnore] public IcpcCode IcpcCode { get; set; }
    }
}