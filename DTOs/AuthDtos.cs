namespace Rest_API_service.DTOs;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record RefreshRequest(string RefreshToken);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);
