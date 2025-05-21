namespace Contract.Dtos.Authentication.Responses;

public record AuthResponse(string Token, Guid UserId, string UserStatus);