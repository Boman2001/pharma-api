namespace Core.Domain
{
    public class Activity : BaseEntity
    {
        public string Description { get; set; }
        public string Properties { get; set; }
        
        public int SubjectId { get; set; }
        public BaseEntity SubjectType { get; set; }
        
        public int CauserId { get; set; }
        public BaseEntity CauserType { get; set; }
    }
}