using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.IcpcCodes
{
    public class BaseIcpcCodeDto
    {
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Code is verplicht.")]
        public string Code { get; set; }
    }
}