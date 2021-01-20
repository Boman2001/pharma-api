using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Intolerances
{
    public class IntoleranceDto : BaseIntoleranceDto
    {
        public int Id { get; set; }
        
        public ConsultationDto Consultation { get; set; }
        public PatientDto Patient { get; set; }
    }
}