using System;

namespace Core.Domain
{
    public abstract class BaseEntitySoftDeletes : BaseEntity, IBaseEntitySoftDeletes
    {
        public DateTime? DeletedAt { get; set; }
        public DateTime? DeletedBy { get; set; }
    }
}