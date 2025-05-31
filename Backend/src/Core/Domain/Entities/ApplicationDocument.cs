using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities
{
    public class ApplicationDocument : BaseEntity<Guid>
    {
        public Guid MentorApplicationId { get; set; }
        public FileType DocumentType { get; set; }
        public string DocumentUrl { get; set; } = null!;
        public MentorApplication MentorApplication { get; set; } = null!;
    }
}
