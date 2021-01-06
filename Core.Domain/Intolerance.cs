namespace Core.Domain
{
    public class Intolerance : BaseEntity
    {
        public string Description { get; set; }
        
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }
        
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}