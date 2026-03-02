using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;

namespace AstraVenturaAuth.Core.UseCases;

public sealed class AuthenticateUserUseCase : IAuthenticateUserUseCase
{
    #region Puertos
    // Peristencia de los usuarios
    private readonly IUserRepository _users;
    // Generación de la identidad del usuario
    private readonly ITokenGenerator _tokens;
    // Cifrado de contraseñas
    private readonly IPasswordHasher _hasher;

    #endregion

    public AuthenticateUserUseCase(IUserRepository users, ITokenGenerator tokens, IPasswordHasher hasher)
    {
        _users  = users;
        _tokens = tokens;
        _hasher = hasher;
    }

    public async Task<Result<AuthenticatedUserDto>> ExecuteAsync(CredentialsDto dto, CancellationToken ct = default)
    {
        // Manejo de error básico por email o password faltantes
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidCredentials);

        var password = new Password(dto.Password);
        var email = new Email(dto.Email);
        var user  = await _users.FindByEmailAsync(email, ct);

        // Validando si existe el usuario y verificando si la contraseña es correcta
        if (user is null || !_hasher.Verify(password.Value, user.PasswordHash.Value))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidCredentials);

        // Generando el par de tokens
        var tokenPair = _tokens.Generate(user);

        // Retornando el usuario autenticado
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
