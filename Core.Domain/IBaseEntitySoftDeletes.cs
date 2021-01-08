using System;

namespace Core.Domain
{
    public interface IBaseEntitySoftDeletes
    {
        public DateTime? DeletedAt { get; set; }
        public DateTime? DeletedBy { get; set; }
    }
}