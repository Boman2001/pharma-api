namespace Core.Domain
{
    public class PhysicalExamination : BaseEntity
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }
        
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }
        
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        public int ExaminationTypeId { get; set; }
        public ExaminationType ExaminationType { get; set; }
    }
}
