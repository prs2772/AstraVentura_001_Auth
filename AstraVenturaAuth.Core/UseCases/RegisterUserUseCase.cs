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
    #region Puertos
    // Peristencia de los usuarios
    private readonly IUserRepository _users;

    // Generación de la identidad del usuario
    private readonly ITokenGenerator _tokens;

    // Cifrado de contraseñas
    private readonly IPasswordHasher _hasher;

    #endregion

    public RegisterUserUseCase(
        IUserRepository users,
        ITokenGenerator tokens,
        IPasswordHasher hasher
    )
    {
        _users = users;
        _tokens = tokens;
        _hasher = hasher;
    }

    /// <summary>
    /// Gets a new User to register, validates, inserts and generates a new pair of tokens to return if success
    /// </summary>
    /// <param name="dto">RegisterNewUserDto with all the required data</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Result<AuthenticatedUserDto> with the user data and tokens if success, otherwise an error and details</returns>
    public async Task<Result<AuthenticatedUserDto>> ExecuteAsync(
        RegisterNewUserDto dto,
        CancellationToken ct = default
    )
    {
        // Validando que el Email que recibimos no venga vacío ni el password
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidCredentials);

        Password password;
        Email email;

        try
        {
            password = new Password(dto.Password);
            email = new Email(dto.Email);
        }
        catch (ArgumentException ex)
        {
            // Los Value Objects lanzan ArgumentException con el mensaje de validación
            var validationError = new ErrorResult("AVAuth.ValidationError", ex.Message);
            return Result<AuthenticatedUserDto>.Failure(validationError);
        }

        if (await _users.ExistsAsync(email, ct))
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.EmailAlreadyTaken);

        var name = new PersonName(
            firstName: dto.Nombre,
            lastName: dto.ApellidoPrincipal,
            secondLastName: dto.ApellidoSecundario
        );

        var hash = _hasher.Hash(password.Value);
        var user = new User(UserId.New(), email, name, new PasswordHash(hash));

        var saveResult = await _users.SaveAsync(user, ct);
        if (saveResult.IsFailure)
            return Result<AuthenticatedUserDto>.Failure(saveResult.Error);

        var tokenPair = await _tokens.GenerateAsync(user);

        return Result<AuthenticatedUserDto>.Success(
            new AuthenticatedUserDto
            {
                Id = user.Id.ToString(),
                Email = user.EmailAddress.Value,
                Name = user.Name.FullName,
                AccessToken = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                ExpiresAt = tokenPair.ExpiresAt,
            }
        );
    }
}
