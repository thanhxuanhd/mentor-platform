using Application.Services.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Contract.Dtos.Messages.Requests;
using Microsoft.AspNetCore.Authorization;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllUserConversationsAsync(int pageIndex)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await messageService.GetListConversationsByUserId(userId, pageIndex);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("conversations/{conversationId}")]
    public async Task<IActionResult> GetAllUserConversationsAsync(Guid conversationId, int pageIndex)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await messageService.GetConversationMessageHistory(userId, conversationId, pageIndex);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("conversations/filter")]
    public async Task<IActionResult> GetSearchConversationAsync([FromQuery] GetFilterConversationRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await messageService.GetConversationsBySearch(userId, request);

        return StatusCode((int)result.StatusCode, result);
    }
}