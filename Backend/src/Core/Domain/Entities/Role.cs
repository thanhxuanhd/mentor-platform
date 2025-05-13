using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Role : BaseEntity<int>
{
    public UserRole Name { get; set; }
    public ICollection<User>? Users { get; set; }
}