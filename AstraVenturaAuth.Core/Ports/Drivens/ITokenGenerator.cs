using AstraVenturaAuth.Core.Domain;

namespace AstraVenturaAuth.Core.Ports.Drivens;

public record TokenPair(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public interface ITokenGenerator
{
    Task<TokenPair?> GenerateAsync(User user, CancellationToken ct = default);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    // El UserId lo necesitamos para emitir un nuevo par de tokens sin volver a autenticar
    Task<string?> GetUserIdFromRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default
    );
    Task InvalidateAllRefreshTokensForUserAsync(string userId, CancellationToken ct = default);
}
