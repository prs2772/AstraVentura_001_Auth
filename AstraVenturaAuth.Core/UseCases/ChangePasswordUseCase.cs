using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;

namespace AstraVenturaAuth.Core.UseCases;

public sealed class ChangePasswordUseCase(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher
) : IChangePasswordUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<bool>> ExecuteAsync(
        string userIdStr,
        ChangePasswordDto dto,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        if (dto.CurrentPassword == dto.NewPassword)
        {
            return Result<bool>.Failure(
                new ErrorResult(
                    "IdenticalPassword",
                    "The new password cannot be the same as the current password."
                )
            );
        }

        var userId = new UserId(userIdGuid);
        var user = await _userRepository.FindByIdAsync(userId, ct);

        if (user == null)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        if (!_passwordHasher.Verify(dto.CurrentPassword, user.PasswordHash.Value))
        {
            return Result<bool>.Failure(AuthErrors.InvalidCredentials);
        }

        var hashedNewPassword = _passwordHasher.Hash(dto.NewPassword);
        user.UpdatePassword(new PasswordHash(hashedNewPassword));

        await _userRepository.UpdateAsync(user, ct);
        await _tokenGenerator.InvalidateAllRefreshTokensForUserAsync(user.Id.ToString(), ct);

        return Result<bool>.Success(true);
    }
}
