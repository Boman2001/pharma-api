using System.Text.Json.Serialization;
using Core.Domain.Interfaces;

namespace Core.Domain.Models
{
    public class UserJournal : BaseEntity
    {
        public string Description { get; set; }
        public UserJournalType Property { get; set; }

        public int ConsultationId { get; set; }
        [JsonIgnore] public Consultation Consultation { get; set; }

        public int PatientId { get; set; }
        [JsonIgnore] public Patient Patient { get; set; }
    }
}