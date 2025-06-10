using Application.Services.Messages;
using Contract.Dtos.Messages.Requests;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MentorPlatformAPI.Hubs;

public class MessageHub(IMessageService messageService) : Hub
{
    public async Task SendMessage(AddMessageRequest request)
    {
        var senderId = Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await messageService.AddMessageAsync(senderId, request);
        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        var response = result.Value;
        var lastMessage = response!.LastMessage;
        if (lastMessage != null)
        {
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