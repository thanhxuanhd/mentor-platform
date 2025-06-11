using Application.Services.Messages;
using Contract.Dtos.Messages.Requests;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MentorPlatformAPI.Hubs;

[Authorize]
public class MessageHub(IMessageService messageService, ILogger<MessageHub> logger) : Hub
{
    public async Task SendMessage(SendMessageRequest request)
    {
        logger.LogInformation("Start SendMessage with request: {@Request}", request);
        var senderIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (senderIdClaim == null) throw new InvalidOperationException("Sender ID claim not found.");
        var senderId = Guid.Parse(senderIdClaim.Value);

        Guid? conversationId = string.IsNullOrEmpty(request.ConversationId) || request.ConversationId == "null"
            ? null
            : Guid.TryParse(request.ConversationId, out var cid) ? cid : null;
        Guid? recipientId = string.IsNullOrEmpty(request.RecipientId) || request.RecipientId == "null"
            ? null
            : Guid.TryParse(request.RecipientId, out var rid) ? rid : null;

        var adjustedRequest = new AddMessageRequest(conversationId, recipientId, request.Content);

        var result = await messageService.AddMessageAsync(senderId, adjustedRequest);
        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        var response = result.Value;
        var lastMessage = response!.LastMessage;
        if (lastMessage != null)
        {
            logger.LogInformation("Broadcasting ReceiveMessage for ConversationId: {ConversationId}", response.ConversationId);
            await Clients.Group(response.ConversationId.ToString() ?? "default-group").SendAsync(
                "ReceiveMessage",
                lastMessage.SenderId.ToString(),
                lastMessage.Content,
                lastMessage.MessageId.ToString(),
                lastMessage.SenderName,
                lastMessage.SenderProfilePhotoUrl,
                lastMessage.SentAt,
                response.ConversationId.ToString() ?? "default-group"
            );
        }
    }
}