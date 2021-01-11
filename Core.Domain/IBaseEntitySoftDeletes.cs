using System;

namespace Core.Domain
{
    public interface IBaseEntitySoftDeletes
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}