using System;

namespace Core.Domain.Models
{
    public class Prescription : BaseEntity
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
