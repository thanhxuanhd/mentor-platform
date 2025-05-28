using Domain.Enums;

namespace Contract.Dtos.MentorApplication.Requests;

public class UpdateMentorApplicationRequest
{
    public List<UpdateDocument>? Documents { get; set; }
}

public class UpdateDocument
{
    public Guid DocumentId { get; set; }
    public FileType DocumentType { get; set; }
    public string DocumentUrl { get; set; } = null!;
}
