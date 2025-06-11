using Application.Services.Messages;
using Contract.Dtos.Messages.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace MentorPlatformAPI.Hubs;

[Authorize]
public class MessageHub(IMessageService messageService, ILogger<MessageHub> logger) : Hub
{
    public static readonly ConcurrentDictionary<string, string> UserConnections = new();

    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        UserConnections[userId] = Context.ConnectionId!;
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;
        UserConnections.TryRemove(userId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        logger.LogInformation("Start SendMessage with request: {@Request}", request);
        var senderIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (senderIdClaim == null)
        {
            throw new InvalidOperationException("Sender ID claim not found.");
        }

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

        foreach (var participant in response.Participants)
        {
            if (UserConnections.TryGetValue(participant.Id.ToString(), out var connId))
            {
                await Groups.AddToGroupAsync(connId, response.ConversationId.ToString());
            }
        }

        if (lastMessage != null)
        {
            logger.LogInformation("Broadcasting ReceiveMessage content {Content} for ConversationId: {ConversationId}", response.LastMessage.Content, response.ConversationId);
            await Clients.Group(response.ConversationId.ToString()).SendAsync(
                "ReceiveMessage",
                lastMessage.SenderId.ToString(),
                lastMessage.Content,
                lastMessage.MessageId.ToString(),
                lastMessage.SenderName,
                lastMessage.SenderProfilePhotoUrl,
                lastMessage.SentAt,
                response.ConversationId.ToString()
            );
        }
    }
}