using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Prescriptions
{
    public class UpdatedPrescriptionDto : BasePrescriptionDto
    {
        public int Id { get; set; }
    }
}