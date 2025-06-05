using System.Runtime.InteropServices;
using Application.Services.LearnerDashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace MentorPlatformAPI.Controllers
{
    [Route("api/dashboards/learner")]
    [ApiController]
    public class LearnerDashboardController(ILearnerDashboardService learnerDashboardService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetLearnerDashboard(Guid userId)
        {
            var result = await learnerDashboardService.GetLearnerDashboardAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
