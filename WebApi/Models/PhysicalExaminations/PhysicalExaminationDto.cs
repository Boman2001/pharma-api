using WebApi.Models.ExaminationTypes;

namespace WebApi.Models.PhysicalExaminations
{
    public class PhysicalExaminationDto : BasePhysicalExaminationDto
    {
        public int Id { get; set; }
        public ExaminationTypeDto ExaminationType { get; set; }
    }
}