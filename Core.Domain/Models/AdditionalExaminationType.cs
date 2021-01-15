using System.Collections.Generic;

namespace Core.Domain.Models
{
    public class AdditionalExaminationType : BaseEntity
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public List<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
    }
}