using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;

namespace AstraVenturaAuth.Core.UseCases;

public sealed class RefreshTokenUseCase : IRefreshTokenUseCase
{
    #region Ports
    private readonly IUserRepository _users;
    private readonly ITokenGenerator _tokens;
    
    #endregion

    public RefreshTokenUseCase(IUserRepository users, ITokenGenerator tokens)
    {
        _users  = users;
        _tokens = tokens;
    }

    public async Task<Result<AuthenticatedUserDto>> ExecuteAsync(string refreshToken, CancellationToken ct = default)
    {
        var isValid = await _tokens.ValidateRefreshTokenAsync(refreshToken, ct);
        if (!isValid)
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidRefreshToken);

        var rawId = await _tokens.GetUserIdFromRefreshTokenAsync(refreshToken, ct);
        if (rawId is null || !Guid.TryParse(rawId, out var guid))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidRefreshToken);

        var user = await _users.FindByIdAsync(new UserId(guid), ct);
        if (user is null)
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.UserNotFound);

        await _tokens.InvalidateRefreshTokenAsync(refreshToken, ct);   // rotación del token
        var tokenPair = _tokens.Generate(user);

        return Result<AuthenticatedUserDto>.Success(new AuthenticatedUserDto
        {
            Id           = user.Id.ToString(),
            Email        = user.EmailAddress.Value,
            Name         = user.Name.FullName,
            AccessToken  = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresAt    = tokenPair.ExpiresAt
        });
    }
}
