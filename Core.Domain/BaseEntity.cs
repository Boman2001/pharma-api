using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain
{
    [NotMapped]
    public abstract class BaseEntity : IEntity
    {
        [Key] public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        [NotMapped]
        public List<Activity> Activities { get; set; }
    }
}