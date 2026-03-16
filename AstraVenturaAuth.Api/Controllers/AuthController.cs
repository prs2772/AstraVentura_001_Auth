using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivers;
using Microsoft.AspNetCore.Mvc;

namespace AstraVenturaAuth.Api.Controllers;

/// <summary>
/// Controlador de autenticación, implementa los puertos de entrada (Drivers)
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    #region Puertos de entrada (Drivers)
    private readonly IAuthenticateUserUseCase _authenticate;
    private readonly IRegisterUserUseCase     _register;
    private readonly IRefreshTokenUseCase     _refresh;

    #endregion

    public AuthController(IAuthenticateUserUseCase authenticate, IRegisterUserUseCase register, IRefreshTokenUseCase refresh)
    {
        _authenticate = authenticate;
        _register = register;
        _refresh = refresh;
    }

    /// <summary>
    /// Inicia sesión
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsDto dto, CancellationToken ct)
    {
        var result = await _authenticate.ExecuteAsync(dto, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterNewUserDto dto, CancellationToken ct)
    {
        var result = await _register.ExecuteAsync(dto, ct);

        return result.IsSuccess
            ? StatusCode(201, result.Value)
            : Conflict(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>
    /// Refresca el token principal con el temporal
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var result = await _refresh.ExecuteAsync(dto.RefreshToken, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error.Code, result.Error.Message });
    }
}
