using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivers;
using AstraVenturaAuth.Core.UseCases;
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
    private readonly IRegisterUserUseCase _register;
    private readonly IRefreshTokenUseCase _refresh;
    private readonly IRequestPasswordRecoveryUseCase _requestPasswordRecovery;
    private readonly IResetPasswordUseCase _resetPassword;

    #endregion

    public AuthController(
        IAuthenticateUserUseCase authenticate,
        IRegisterUserUseCase register,
        IRefreshTokenUseCase refresh,
        IRequestPasswordRecoveryUseCase requestPasswordRecovery,
        IResetPasswordUseCase resetPassword
    )
    {
        _authenticate = authenticate;
        _register = register;
        _refresh = refresh;
        _requestPasswordRecovery = requestPasswordRecovery;
        _resetPassword = resetPassword;
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
    public async Task<IActionResult> Register(
        [FromBody] RegisterNewUserDto dto,
        CancellationToken ct
    )
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

    /// <summary>
    /// Inicia la recuperación de contraseña enviando un correo de reseteo
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto dto,
        CancellationToken ct
    )
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _requestPasswordRecovery.ExecuteAsync(dto.Email, ipAddress, ct);

        // Si devuelve error, podría ser rate-limit excedido (403). De lo contrario éxito anónimo.
        if (result.IsSuccess)
        {
            return Ok(new { Message = "If the email exists, a reset link will be sent." });
        }

        return StatusCode(403, new { result.Error.Code, result.Error.Message });
    }

    /// <summary>
    /// Restablece la contraseña de usuario con un token válido
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto,
        CancellationToken ct
    )
    {
        var result = await _resetPassword.ExecuteAsync(dto, ct);

        return result.IsSuccess
            ? Ok(new { Message = "Password has been successfully reset." })
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }
}
