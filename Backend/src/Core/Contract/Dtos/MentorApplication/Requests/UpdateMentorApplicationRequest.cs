using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Contract.Dtos.MentorApplication.Requests;

public class UpdateMentorApplicationRequest
{
    public string? Experiences { get; set; }
    public string? Education { get; set; }
    public string? Certifications { get; set; }
    public string? Statement { get; set; }
    public List<IFormFile>? Documents { get; set; }
}