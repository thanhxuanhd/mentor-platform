using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tag : BaseEntity<uint>
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<CourseTag> CourseTags { get; set; } = new HashSet<CourseTag>();
    }
}
