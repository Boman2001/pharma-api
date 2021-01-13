﻿using System.Collections.Generic;

namespace Core.Domain.Models
{
    public class IcpcCode : BaseEntitySoftDeletes
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public List<Episode> Episodes { get; set; }
    }
}