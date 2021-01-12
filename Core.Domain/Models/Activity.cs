using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Models
{
    public class Activity : BaseEntitySoftDeletes
    {
        public string Description { get; set; }
        public string Properties { get; set; }
        
        public int SubjectId { get; set; }
        public BaseEntity SubjectType { get; set; }
        
        public Guid CauserId { get; set; }
        [NotMapped]
        public IdentityUser CauserType { get; set; }
    }
}