using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Ports.Drivens;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AstraVenturaAuth.Adapters.Drivens.Security;

/// <summary>
/// Generador de tokens JWT, implementa el puerto ITokenGenerator
/// </summary>
public sealed class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _options;
    private static readonly Dictionary<string, (string UserId, DateTime Expiry)> _refreshTokenStore = new();

    // Se inyectan las opciones de appsettings.json
    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Genera un par de tokens (Access Token y Refresh Token)
    /// </summary>
    public TokenPair Generate(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);
        var accessToken = BuildJwt(user, expiresAt);
        var refreshToken = GenerateRefreshToken();

        var refreshExpiry = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        _refreshTokenStore[refreshToken] = (user.Id.ToString(), refreshExpiry);

        return new TokenPair(accessToken, refreshToken, expiresAt);
    }

    /// <summary>
    /// Valida un refresh token
    /// </summary>
    public Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var isValid = _refreshTokenStore.TryGetValue(refreshToken, out var entry)
                      && entry.Expiry > DateTime.UtcNow;

        return Task.FromResult(isValid);
    }

    /// <summary>
    /// Invalida un refresh token que ya se haya usado para solicitar otro
    /// </summary>
    public Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        _refreshTokenStore.Remove(refreshToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene el ID de usuario del refresh token
    /// </summary>
    public Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        _refreshTokenStore.TryGetValue(refreshToken, out var entry);
        var userId = entry.Expiry > DateTime.UtcNow ? entry.UserId : null;
        return Task.FromResult(userId);
    }

    #region Helpers privados
    
    /// <summary>
    /// Construye el Access Token, tomando en cuenta la key secreta, fecha de expiración y claims de usuario
    /// </summary>
    private string BuildJwt(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
