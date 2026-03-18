using Rest_API_service.Models;

namespace Rest_API_service.Services;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
