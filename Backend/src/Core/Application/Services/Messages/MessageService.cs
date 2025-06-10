using System.Net;
using Contract.Dtos.Messages.Requests;
using Contract.Dtos.Messages.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Messages;

public class MessageService(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    IUserRepository userRepository,
    IConversationParticipantRepository conversationParticipantRepository) : IMessageService
{
    public async Task<Result<GetMinimalConversationResponse>> AddMessageAsync(Guid senderId, AddMessageRequest request)
    {
        var newConversation = new Conversation();
        if (request.ConversationId is null)
        {
            var user = await userRepository.GetByIdAsync(request.RecipientId!.Value);
            if (user == null)
            {
                return Result.Failure<GetMinimalConversationResponse>("Recipient not found", HttpStatusCode.BadRequest);
            }
            newConversation = new Conversation
            {
                CreatedAt = DateTime.UtcNow,
                Name = user.FullName
            };
            await conversationRepository.AddAsync(newConversation);
            await conversationRepository.SaveChangesAsync();

            var conversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant
                {
                    ConversationId = newConversation.Id,
                    UserId = senderId,
                    IsAdmin = true
                },
                new ConversationParticipant
                {
                    ConversationId = newConversation.Id,
                    UserId = request.RecipientId.Value,
                    IsAdmin = true
                },
            };

            await conversationParticipantRepository.AddRangeAsync(conversationParticipants);
            await conversationParticipantRepository.SaveChangesAsync();
        }

        var targetConversation = request.ConversationId ?? newConversation.Id;
        var conversation = await conversationRepository.GetByIdAsync(targetConversation);
        if (conversation == null)
        {
            return Result.Failure<GetMinimalConversationResponse>("Conversation not found", HttpStatusCode.BadRequest);
        }

        var message = new Message
        {
            ConversationId = request.ConversationId!.Value,
            SenderId = senderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        await messageRepository.AddAsync(message);
        await messageRepository.SaveChangesAsync();

        var response = new GetMinimalConversationResponse
        {
            ConversationId = conversation.Id,
            ConversationName = conversation.Name,
            LastMessage = new GetMessageResponse
            {
                Content = message.Content,
                SentAt = message.SentAt,
                MessageId = message.Id,
                SenderId = message.SenderId,
                SenderName = message.Sender.FullName,
                SenderProfilePhotoUrl = message.Sender.ProfilePhotoUrl ?? string.Empty,
            }
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<GetMinimalConversationResponse>>> GetListConversationsByUserId(Guid userId, int pageIndex)
    {
        var conversationsQuery = conversationRepository.GetAllInclude()
            .Where(conv => conv.Participants.Any(p => p.UserId == userId))
            .Select(conv => new GetMinimalConversationResponse
            {
                ConversationId = conv.Id,
                ConversationName = conv.Name,
                Participants = conv.Participants.Select(p => new ConversationParticipantResponse
                {
                    Id = p.User.Id,
                    FullName = p.User.FullName,
                    ProfilePhotoUrl = p.User.ProfilePhotoUrl ?? string.Empty,
                    Role = p.User.Role.Name.ToString()
                }).ToList(),
                LastMessage = conv.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new GetMessageResponse
                    {
                        Content = m.Content,
                        SentAt = m.SentAt,
                        MessageId = m.Id,
                        SenderId = m.SenderId,
                        SenderName = m.Sender.FullName,
                        SenderProfilePhotoUrl = m.Sender.ProfilePhotoUrl ?? string.Empty,
                    })
                    .FirstOrDefault() ?? null
            });

        var paginatedConversations = await conversationRepository.ToPaginatedListAsync(conversationsQuery, 10, pageIndex);

        return Result.Success(paginatedConversations, HttpStatusCode.OK);
    }

    public async Task<Result<GetDetailConversationResponse>> GetConversationMessageHistory(Guid userId, Guid conversationId, int pageIndex)
    {
        var conversation = await conversationRepository.GetByIdAsync(conversationId, conv => conv.Participants.Any(p => p.UserId == userId));
        if (conversation == null)
        {
            return Result.Failure<GetDetailConversationResponse>("Conversation not found", HttpStatusCode.BadRequest);
        }

        var messagesQuery = messageRepository.GetAll()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Select(m => new GetMessageResponse
            {
                Content = m.Content,
                SentAt = m.SentAt,
                MessageId = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.FullName,
                SenderProfilePhotoUrl = m.Sender.ProfilePhotoUrl ?? string.Empty,
            });

        var paginatedMessages = await messageRepository.ToPaginatedListAsync(messagesQuery, 10, pageIndex);

        var result = new GetDetailConversationResponse
        {
            ConversationId = conversation.Id,
            ConversationName = conversation.Name,
            Messages = paginatedMessages,
        };

        return Result.Success(result, HttpStatusCode.OK);
    }

    public async Task<Result<List<GetFilterConversationResponse>>> GetConversationsBySearch(Guid userId, GetFilterConversationRequest request)
    {
        var keyword = request.Keyword?.Trim() ?? string.Empty;

        // Query users (up to 5)
        var usersQuery = userRepository.GetAll()
            .Where(u => u.Status == UserStatus.Active && u.IsAllowedMessage &&
                       (string.IsNullOrEmpty(keyword) || u.FullName.Contains(keyword)))
            .Select(u => new
            {
                User = u,
                ConversationId = conversationRepository.GetAllInclude()
                    .Where(c => c.Participants.Count == 2 &&
                                c.Participants.Any(p => p.UserId == userId) &&
                                c.Participants.Any(p => p.UserId == u.Id))
                    .Select(c => c.Id)
                    .FirstOrDefault(),
                HasConversation = conversationRepository.GetAllInclude()
                    .Any(c =>c.Participants.Any(p => p.UserId == userId) &&
                              c.Participants.Any(p => p.UserId == u.Id))
            })
            .OrderByDescending(x => x.HasConversation)
            .ThenBy(x => x.User.FullName)
            .Take(5);

        // Query groups (up to 3)
        var groupsQuery = conversationRepository.GetAllInclude()
            .Where(c => c.Participants.Any(p => p.UserId == userId) &&
                       (string.IsNullOrEmpty(keyword) || c.Name.Contains(keyword)))
            .Select(c => new
            {
                Conversation = c,
                HasConversation = true
            })
            .OrderBy(c => c.Conversation.Name)
            .Take(3);

        // Combine queries
        var combinedQuery = usersQuery
            .Select(x => new GetFilterConversationResponse
            {
                Id = x.ConversationId != Guid.Empty ? x.ConversationId : x.User.Id,
                PhotoUrl = x.User.ProfilePhotoUrl ?? string.Empty,
                Name = x.User.FullName,
                ConversationId = x.ConversationId != Guid.Empty ? x.ConversationId : null,
                IsGroup = false
            })
            .Concat(groupsQuery.Select(x => new GetFilterConversationResponse
            {
                Id = x.Conversation.Id,
                PhotoUrl = string.Empty,
                Name = x.Conversation.Name,
                ConversationId = x.Conversation.Id,
                IsGroup = true
            }))
            .OrderBy(x => x.IsGroup)
            .ThenByDescending(x => x.ConversationId.HasValue)
            .ThenBy(x => x.Name);

        var results = await conversationRepository.ToListAsync(combinedQuery);

        return Result.Success(results, HttpStatusCode.OK);
    }
}