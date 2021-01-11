using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain
{
    public class Activity : BaseEntitySoftDeletes
    {
        public string Description { get; set; }
        public string Properties { get; set; }
        
        public int SubjectId { get; set; }
        public string SubjectType { get; set; }
        
        public Guid CauserId { get; set; }
    }
}