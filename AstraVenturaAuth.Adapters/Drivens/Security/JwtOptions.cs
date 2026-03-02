namespace AstraVenturaAuth.Adapters.Drivens.Security;

/// <summary>
/// Opciones de configuración para JWT, Patrón Options (Options Pattern). Tipa fuertemente la configuracion del settings
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}
