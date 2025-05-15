using Application.Services.Courses;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(ICourseService service) : ControllerBase
{
    [HttpGet("/")]
    public async Task<IActionResult> GetAllCourses()
    {
        var items = await service.GetAllAsync();
    }
    
    [HttpGet("/{id}")]
    public async Task<IActionResult> GetCourseById()
    {
       
    }
    
    [HttpPost("/")]
    public async Task<IActionResult> CreateCourse()
    {
       
    }
    
    [HttpPost("/{id}")]
    public async Task<IActionResult> UpdateCourse()
    {
       
    }
}