using System;

namespace Core.Domain
{
    public class AdditionalExaminationResult : BaseEntity
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int AdditionalExaminationTypeId { get; set; }
        public AdditionalExaminationType AdditionalExaminationType { get; set; }
    }
}