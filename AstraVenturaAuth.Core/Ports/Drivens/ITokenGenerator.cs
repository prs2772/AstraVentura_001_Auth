using AstraVenturaAuth.Core.Domain;

namespace AstraVenturaAuth.Core.Ports.Drivens;

public record TokenPair(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public interface ITokenGenerator
{
    TokenPair Generate(User user);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    // El UserId lo necesitamos para emitir un nuevo par sin re-autenticar
    Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
