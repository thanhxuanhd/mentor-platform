using Application.Services.CourseResources;
using Contract.Dtos.CourseResources.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MentorPlatformAPI.Controllers
{
    [Route("api/Resources")]
    [ApiController]
    public class CourseResourcesController(ICourseResourceService courseResourceService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCourseResources([FromQuery] FilterResourceRequest request)
        {
            request.Keyword = string.IsNullOrEmpty(request.Keyword) ? string.Empty : request.Keyword.Trim();
            var serviceResult = await courseResourceService.FilterResourceAsync(request);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        [Authorize]
        [HttpGet("{courseResourceId}")]
        public async Task<IActionResult> GetCourseResourceById(Guid courseResourceId)
        {
            var serviceResult = await courseResourceService.GetByIdAsync(courseResourceId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> CreateCourseResource([FromForm] CourseResourceRequest formData)
        {
            var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var serviceResult = await courseResourceService.CreateAsync(mentorId, formData.CourseId, formData, Request);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        [HttpPut("{courseResourceId}")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> UpdateCourseResource(Guid courseResourceId, [FromForm] CourseResourceRequest formData)
        {
            var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var serviceResult = await
                courseResourceService.UpdateAsync(mentorId, courseResourceId, formData, Request);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        [HttpDelete("{courseResourceId}")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> DeleteCourseResource(Guid courseResourceId)
        {
            var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var serviceResult = await courseResourceService.DeleteAsync(mentorId, courseResourceId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        [Authorize]
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFileAsync(Guid courseResourceId, string fileName)
        {
            try
            {
                var serviceResult = await courseResourceService.DownloadFileAsync(courseResourceId, fileName);

                return serviceResult;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            when (ex is KeyNotFoundException
            or FileNotFoundException
            or DirectoryNotFoundException)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
