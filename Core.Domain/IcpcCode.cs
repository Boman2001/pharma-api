namespace Core.Domain
{
    public class IcpcCode: BaseEntitySoftDeletes
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}