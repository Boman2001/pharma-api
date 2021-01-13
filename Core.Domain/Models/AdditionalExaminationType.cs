using System.Collections.Generic;

namespace Core.Domain.Models
{
    public class AdditionalExaminationType : BaseEntitySoftDeletes
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public List<AdditionalExaminationResult> AdditionalExaminationResults { get; set; }
    }
}