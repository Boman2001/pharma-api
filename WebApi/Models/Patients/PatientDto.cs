namespace WebApi.Models.Patients
{
    public class PatientDto : BasePatientDto
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}