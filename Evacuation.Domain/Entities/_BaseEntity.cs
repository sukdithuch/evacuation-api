namespace Evacuation.Domain.Entities
{
    public class BaseEntity : IBaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; }
    }
}
