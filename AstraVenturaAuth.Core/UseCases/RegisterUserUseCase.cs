using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;

namespace AstraVenturaAuth.Core.UseCases;

public sealed class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserRepository _users;
    private readonly ITokenGenerator _tokens;
    private readonly IPasswordHasher _hasher;

    public RegisterUserUseCase(IUserRepository users, ITokenGenerator tokens, IPasswordHasher hasher)
    {
        _users = users;
        _tokens = tokens;
        _hasher = hasher;
    }

    public async Task<Result<AuthenticatedUserDto>> ExecuteAsync(RegisterNewUserDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidCredentials);

        var email = new Email(dto.Email);

        if (await _users.ExistsAsync(email, ct))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.EmailAlreadyTaken);

        var name = new PersonName(
            firstName: dto.Nombre,
            lastName: dto.ApellidoPrincipal,
            secondLastName: dto.ApellidoSecundario);

        var hash = _hasher.Hash(dto.Password);
        var user = new User(UserId.New(), email, name, new PasswordHash(hash));

        var saveResult = await _users.SaveAsync(user, ct);
        if (saveResult.IsFailure)
            return Result<AuthenticatedUserDto>.Failure(saveResult.Error);

        var tokenPair = _tokens.Generate(user);

        return Result<AuthenticatedUserDto>.Success(new AuthenticatedUserDto
        {
            Id = user.Id.ToString(),
            Email = user.EmailAddress.Value,
            Name = user.Name.FullName,
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresAt = tokenPair.ExpiresAt
        });
    }
}
