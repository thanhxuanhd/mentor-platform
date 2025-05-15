namespace Contract.Dtos.Categories.Responses;

public class FilterCourseByCategoryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Difficulty { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}
