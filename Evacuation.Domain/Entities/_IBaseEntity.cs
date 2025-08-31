namespace Evacuation.Domain.Entities
{
    public interface IBaseEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        bool Active { get; set; }
    }
}
