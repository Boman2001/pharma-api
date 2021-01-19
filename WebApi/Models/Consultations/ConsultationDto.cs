using WebApi.Models.Patients;

namespace WebApi.Models.Consultations
{
    using Users;

    public class ConsultationDto : BaseConsultationDto
    {
        public int Id { get; set; }
        public UserDto Doctor { get; set; }
        public PatientDto Patient { get; set; }
    }
}