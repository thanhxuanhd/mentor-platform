using Domain.Abstractions;

namespace Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
