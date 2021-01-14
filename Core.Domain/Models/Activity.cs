using System;

namespace Core.Domain.Models
{
    public class Activity : BaseEntity
    {
        public string Description { get; set; }
        public string Properties { get; set; }

        public int SubjectId { get; set; }
        public string SubjectType { get; set; }

        public Guid CauserId { get; set; }
    }
}