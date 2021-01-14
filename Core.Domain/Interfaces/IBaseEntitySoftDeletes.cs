using System;

namespace Core.Domain.Interfaces
{
    public interface IBaseEntitySoftDeletes
    {
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}