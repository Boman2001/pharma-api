namespace Core.Domain
{
    public class Episode : BaseEntity
    {
        public string Description { get; set; }
        public int Priority { get; set; }
        
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }
        
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        public int IcpcCodeId { get; set; }
        public IcpcCode IcpcCode { get; set; }
    }
}
