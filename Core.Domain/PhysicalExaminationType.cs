using System.Collections.Generic;

namespace Core.Domain
{
    public class PhysicalExaminationType: BaseEntitySoftDeletes
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public List<PhysicalExamination> PhysicalExaminations { get; set; }
    }
}