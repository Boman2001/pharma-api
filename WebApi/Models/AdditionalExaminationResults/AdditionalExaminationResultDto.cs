using WebApi.Models.AdditionalExaminationTypes;

namespace WebApi.Models.AdditionalExaminationResults
{
    public class AdditionalExaminationResultDto : BaseAdditionalExaminationResultDto
    {
        public int Id { get; set; }
        public AdditionalExaminationTypeDto AdditionalExaminationType { get; set; }
    }
}