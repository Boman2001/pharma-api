namespace Core.Domain.Models
{
    public class UserJournal : BaseEntity
    {
        public string Description { get; set; }
        public int Property { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}