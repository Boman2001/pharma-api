using WebApi.Models.Patients;
using WebApi.Models.Users;

namespace WebApi.Models.Consultations
{
    public class ConsultationDto : BaseConsultationDto
    {
        public int Id { get; set; }
        public UserDto Doctor { get; set; }
        public PatientDto Patient { get; set; }
    }
}