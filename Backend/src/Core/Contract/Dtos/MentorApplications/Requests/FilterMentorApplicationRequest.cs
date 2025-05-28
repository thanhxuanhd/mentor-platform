using Domain.Enums;

namespace Contract.Dtos.MentorApplications.Requests;

public class FilterMentorApplicationRequest
{
    public string? Keyword { get; set; }
    public ApplicationStatus? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
