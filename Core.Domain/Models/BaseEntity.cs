using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Interfaces;

namespace Core.Domain.Models
{
    [NotMapped]
    public abstract class BaseEntity : IEntity
    {
        [NotMapped] public List<Activity> Activities { get; set; }

        [Key] public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}