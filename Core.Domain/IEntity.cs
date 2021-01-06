using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain
{
    public interface IEntity
    {
        [Key] public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime CreatedBy { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        public DateTime UpdatedBy { get; set; }
        
        public DateTime DeletedAt { get; set; }
        public DateTime DeletedBy { get; set; }
    }
}