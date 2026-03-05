using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rest_API_service.Data;
using Rest_API_service.DTOs;
using Rest_API_service.Models;
using Rest_API_service.Options;

namespace Rest_API_service.Services;

public class AuthService(
    AppDbContext dbContext,
    ITokenService tokenService,
    IPasswordService passwordService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = passwordService.Hash(request.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!passwordService.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        var refreshToken = await dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (refreshToken.IsRevoked || refreshToken.ExpiresAtUtc <= DateTime.UtcNow || refreshToken.User is null)
        {
            throw new UnauthorizedAccessException("Refresh token expired or revoked.");
        }

        refreshToken.IsRevoked = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(refreshToken.User, cancellationToken);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, expiresAtUtc) = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays)
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, refreshToken, expiresAtUtc);
    }
}
