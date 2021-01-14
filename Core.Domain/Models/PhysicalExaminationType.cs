﻿using System.Collections.Generic;

namespace Core.Domain.Models
{
    public class PhysicalExaminationType : BaseEntity
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public List<PhysicalExamination> PhysicalExaminations { get; set; }
    }
}