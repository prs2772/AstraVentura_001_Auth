using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Ports.Drivens;
using Microsoft.Extensions.Caching.Distributed; // 20260301 prs - Se agrega el driver de Redis para cacheo de tokens
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AstraVenturaAuth.Adapters.Drivens.Security;

/// <summary>
/// Generador de tokens JWT, implementa el puerto ITokenGenerator
/// </summary>
public sealed class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _options;
    private readonly IDistributedCache _cache; // 20260301 prs - Se agrega el driver de Redis para cacheo de tokens

    // Se inyectan las opciones de appsettings.json
    public JwtTokenGenerator(IOptions<JwtOptions> options, IDistributedCache cache)
    {
        _options = options.Value;
        _cache = cache;
    }

    /// <summary>
    /// Genera un par de tokens (Access Token y Refresh Token)
    /// </summary>
    public async Task<TokenPair?> GenerateAsync(User user, CancellationToken ct = default)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);
        var accessToken = BuildJwt(user, expiresAt);
        var refreshToken = GenerateRefreshToken();

        // 20260301 prs - Se agrega el almacenamiento en redis y borrado automático en los dias configurados, pasa a ser async
        // La clave RefreshToken con valor de UserId
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_options.RefreshTokenExpirationDays)
        };

        await _cache.SetStringAsync(refreshToken, user.Id.ToString(), cacheOptions, ct);

        return new TokenPair(accessToken, refreshToken, expiresAt);
    }

    /// <summary>
    /// Valida un refresh token con Redis
    /// </summary>
    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var userId = await _cache.GetStringAsync(refreshToken, ct);
        
        // Si Redis devuelve null, significa que no existe o YA EXPIRÓ. Redis se encarga de validar la fecha
        return userId is not null;
    }

    /// <summary>
    /// Invalida un refresh token que ya se haya usado para solicitar otro
    /// </summary>
    public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(refreshToken, ct);
    }

    /// <summary>
    /// Obtiene el ID de usuario del refresh token
    /// </summary>
    public async Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        // Recuperamos el UserId guardado
        return await _cache.GetStringAsync(refreshToken, ct);
    }

    #region Helpers privados
    
    /// <summary>
    /// Construye el Access Token, tomando en cuenta la key secreta, fecha de expiración y claims de usuario
    /// </summary>
    private string BuildJwt(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims estándard de RFC 7519, sub subject (quién eres?) y jti (identificador único del token JWT ID, bloqueable)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress.Value),
            new Claim(JwtRegisteredClaimNames.Name, user.Name.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token, tomando un RandomNumberGenerator llenando el arreglo de 64 bytes para una contraseña temporal segura (usa criptografía)
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    #endregion
}
