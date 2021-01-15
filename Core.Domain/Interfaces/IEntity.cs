using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Interfaces
{
    public interface IEntity
    {
        [Key] public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}