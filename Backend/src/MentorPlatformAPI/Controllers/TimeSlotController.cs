// using Application.Services.MentorTimeSlot;
// using Contract.Dtos.Timeslot.Request;
// using Microsoft.AspNetCore.Mvc;

// namespace MentorPlatformAPI.Controllers;

// [Route("api/[controller]")]
// [ApiController]
// public class TimeSlotController : ControllerBase
// {
//     private readonly ITimeSlotService _timeSlotService;

//     public TimeSlotController(ITimeSlotService timeSlotService)
//     {
//         _timeSlotService = timeSlotService;
//     }

//     //[HttpGet("calendar")]
//     //public async Task<IActionResult> GetWeeklyCalendar([FromQuery] DateTime weekStartDate, [FromQuery] Guid mentorId)
//     //{
//     //    if (mentorId == Guid.Empty)
//     //        return BadRequest("MentorId is required");

//     //    var result = await _timeSlotService.GetWeeklyCalendarAsync(weekStartDate, mentorId);
//     //    return StatusCode((int)result.StatusCode, result);
//     //}

//     //[HttpGet("by-date")]
//     //public async Task<IActionResult> GetTimeSlotsByDate([FromQuery] DateTime date, [FromQuery] Guid mentorId)
//     //{
//     //    if (mentorId == Guid.Empty)
//     //        return BadRequest("MentorId is required");

//     //    var result = await _timeSlotService.GetTimeSlotsByDateAsync(date, mentorId);
//     //    return StatusCode((int)result.StatusCode, result);
//     //}

//     [HttpGet]
//     public async Task<IActionResult> GetAll()
//     {
//         var result = await _timeSlotService.GetAllAsync();
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetTimeslotById(Guid id)
//     {
//         var result = await _timeSlotService.GetTimeslotByIdAsync(id);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpPost("save")]
//     public async Task<IActionResult> SaveAvailability([FromBody] SaveWeeklyAvailabilityRequest request)
//     {
//         if (request == null)
//         {
//             return BadRequest("Request body is required");
//         }

//         var result = await _timeSlotService.SaveWeeklyAvailabilityAsync(request);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpPut("work-hours")]
//     public async Task<IActionResult> UpdateWorkHours([FromBody] UpdateWorkHoursRequest request)
//     {
//         if (request == null)
//         {
//             return BadRequest("Request body is required");
//         }

//         var result = await _timeSlotService.UpdateWorkHoursAsync(request);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpGet("work-hours")]
//     public async Task<IActionResult> GetWorkHours([FromQuery] Guid mentorId)
//     {
//         if (mentorId == Guid.Empty)
//         {
//             return BadRequest("MentorId is required");
//         }

//         var result = await _timeSlotService.GetWorkHoursAsync(mentorId);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpPut("session-parameters")]
//     public async Task<IActionResult> UpdateSessionParameters([FromBody] UpdateSessionParametersRequest request)
//     {
//         if (request == null)
//         {
//             return BadRequest("Request body is required");
//         }

//         var result = await _timeSlotService.UpdateSessionParametersAsync(request);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpGet("session-parameters")]
//     public async Task<IActionResult> GetSessionParameters([FromQuery] Guid mentorId)
//         {
//         if (mentorId == Guid.Empty)
//         {
//             return BadRequest("MentorId is required");
//         }

//         var result = await _timeSlotService.GetSessionParametersAsync(mentorId);
//         return StatusCode((int)result.StatusCode, result);
//     }

//     [HttpGet("locked-status")]
//     public async Task<IActionResult> CheckScheduleLocked([FromQuery] Guid mentorId)
//     {
//         if (mentorId == Guid.Empty)
//         {
//             return BadRequest("MentorId is required");
//         }

//         var result = await _timeSlotService.CheckLockedStatusAsync(mentorId);
//         return StatusCode((int)result.StatusCode, result);
//     }
// }