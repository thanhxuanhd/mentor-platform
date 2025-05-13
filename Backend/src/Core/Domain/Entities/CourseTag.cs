using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CourseTag
    {
        public uint CourseId { get; set; }
        public uint TagId { get; set; }
        public virtual Course Course { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
