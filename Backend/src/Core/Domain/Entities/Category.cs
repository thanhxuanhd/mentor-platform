﻿using Domain.Abstractions;
namespace Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Course>? Courses { get; set; }
        public virtual ICollection<UserCategory>? UserCategories { get; set; }
    }
}
