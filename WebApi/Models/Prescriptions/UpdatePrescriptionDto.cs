using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Prescriptions
{
    public class UpdatePrescriptionDto : BasePrescriptionDto
    {
        public int Id { get; set; }
    }
}