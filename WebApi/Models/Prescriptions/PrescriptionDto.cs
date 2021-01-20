using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Models.Prescriptions
{
    public class PrescriptionDto : BasePrescriptionDto
    {
        public int Id { get; set; }
        
        public ConsultationDto Consultation { get; set; }
        public PatientDto Patient { get; set; }
    }
}