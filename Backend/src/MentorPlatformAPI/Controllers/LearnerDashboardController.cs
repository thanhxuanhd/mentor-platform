using System.Security.Claims;
using Application.Services.LearnerDashboard;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers
{
    [Route("api/dashboards/learner")]
    [ApiController]
    public class LearnerDashboardController(ILearnerDashboardService learnerDashboardService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLearnerDashboard()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await learnerDashboardService.GetLearnerDashboardAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("cancel/{sessionBookingId}")]
        public async Task<IActionResult> CancelSessionBooking(Guid sessionBookingId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await learnerDashboardService.CancelSessionBookingAsync(sessionBookingId, userId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("accept/{sessionBookingId}")]
        public async Task<IActionResult> AcceptSessionBooking(Guid sessionBookingId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await learnerDashboardService.AcceptSessionBookingAsync(sessionBookingId, userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
