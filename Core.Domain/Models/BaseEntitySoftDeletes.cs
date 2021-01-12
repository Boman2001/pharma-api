using System;
using Core.Domain.Interfaces;

namespace Core.Domain.Models
{
    public abstract class BaseEntitySoftDeletes : BaseEntity, IBaseEntitySoftDeletes
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}