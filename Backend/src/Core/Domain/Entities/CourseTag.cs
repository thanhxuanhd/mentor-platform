namespace Domain.Entities
{
    public class CourseTag
    {
        public Guid CourseId { get; set; }
        public Guid TagId { get; set; }
        public virtual Course Course { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
