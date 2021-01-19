using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Prescriptions
{
    public class UpdatedPrescriptionDto : BasePrescriptionDto
    {
        [Required(ErrorMessage = "Id is verplicht.")]
        public int Id { get; set; }
    }
}