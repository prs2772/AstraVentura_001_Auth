namespace AstraVenturaAuth.Core.Common.Errors;

/// <summary>
/// Centraliza nuestros errores de autenticación
/// </summary>
public static class AuthErrors
{
    public static readonly ErrorResult InvalidCredentials = new("AVAuth.InvalidCredentials", "Email o contraseña incorrectos.");
    public static readonly ErrorResult UserNotFound = new("AVAuth.UserNotFound", "El usuario no existe.");
    public static readonly ErrorResult EmailAlreadyTaken = new("AVAuth.EmailAlreadyTaken", "El email ya está registrado.");
    public static readonly ErrorResult InvalidRefreshToken = new("AVAuth.InvalidRefreshToken", "El refresh token no es válido o expiró.");
    public static readonly ErrorResult WeakPassword = new("AVAuth.WeakPassword", "La contraseña no cumple los requisitos mínimos.");
}
