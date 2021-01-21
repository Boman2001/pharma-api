using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Intolerances
{
    public class UpdatedIntoleranceDto : BaseIntoleranceDto
    {
        public int Id { get; set; }
    }
}