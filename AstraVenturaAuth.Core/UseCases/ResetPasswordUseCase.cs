using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;
using Microsoft.Extensions.Caching.Distributed;

namespace AstraVenturaAuth.Core.UseCases;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly IDistributedCache _cache;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordUseCase(
        IDistributedCache cache,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher
    )
    {
        _cache = cache;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> ExecuteAsync(ResetPasswordDto dto, CancellationToken ct)
    {
        var tokenKey = $"pwd-reset:{dto.Token}";
        var userIdStr = await _cache.GetStringAsync(tokenKey, ct);

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
        {
            return Result<bool>.Failure(
                new ErrorResult("InvalidToken", "The recovery token is invalid or has expired.")
            );
        }

        var userId = new AstraVenturaAuth.Core.Domain.ValueObjects.UserId(userIdGuid);
        var user = await _userRepository.FindByIdAsync(userId, ct);
        if (user == null)
        {
            return Result<bool>.Failure(
                new ErrorResult(
                    "UserNotFound",
                    "The user associated with this token was not found."
                )
            );
        }

        // Hash a la nueva contraseña
        var hashedPassword = _passwordHasher.Hash(dto.NewPassword);
        user.UpdatePassword(
            new AstraVenturaAuth.Core.Domain.ValueObjects.PasswordHash(hashedPassword)
        );

        // Guardamos cambios
        await _userRepository.UpdateAsync(user, ct);

        // Invalidamos el token (single-use)
        await _cache.RemoveAsync(tokenKey, ct);

        return Result<bool>.Success(true);
    }
}
