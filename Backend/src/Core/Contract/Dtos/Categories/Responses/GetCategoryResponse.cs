namespace Contract.Dtos.Categories.Responses;

public class GetCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int Courses { get; set; }
    public bool Status { get; set; }
}
