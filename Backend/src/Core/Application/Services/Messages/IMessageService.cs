using Contract.Dtos.Messages.Requests;
using Contract.Dtos.Messages.Responses;
using Contract.Shared;

namespace Application.Services.Messages;

public interface IMessageService
{
    Task<Result<GetMinimalConversationResponse>> AddMessageAsync(Guid senderId, AddMessageRequest request);
    Task<Result<PaginatedList<GetMinimalConversationResponse>>> GetListConversationsByUserId(Guid userId, int pageIndex);
    Task<Result<GetDetailConversationResponse>> GetConversationMessageHistory(Guid userId, Guid conversationId, int pageIndex);
    Task<Result<List<GetFilterConversationResponse>>> GetConversationsBySearch(Guid userId, GetFilterConversationRequest request);
}