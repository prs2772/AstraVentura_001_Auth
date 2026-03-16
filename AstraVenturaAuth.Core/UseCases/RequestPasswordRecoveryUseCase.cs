using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace AstraVenturaAuth.Core.UseCases;

public class RequestPasswordRecoveryUseCase : IRequestPasswordRecoveryUseCase
{
    private readonly IDistributedCache _cache;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public RequestPasswordRecoveryUseCase(
        IDistributedCache cache,
        IUserRepository userRepository,
        IEmailSender emailSender,
        IConfiguration configuration
    )
    {
        _cache = cache;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<Result<bool>> ExecuteAsync(
        string email,
        string ipAddress,
        CancellationToken ct
    )
    {
        // Límites
        var ipRateLimitKey = $"ratelimit:ip:{ipAddress}";
        var emailRateLimitKey = $"ratelimit:email:{email}";

        var ipRatelimitRaw = await _cache.GetStringAsync(ipRateLimitKey, ct);
        var emailRatelimitRaw = await _cache.GetStringAsync(emailRateLimitKey, ct);

        int ipAttempts = string.IsNullOrEmpty(ipRatelimitRaw) ? 0 : int.Parse(ipRatelimitRaw);
        int emailAttempts = string.IsNullOrEmpty(emailRatelimitRaw)
            ? 0
            : int.Parse(emailRatelimitRaw);

        if (ipAttempts >= 3 || emailAttempts >= 3)
        {
            // Note: In a real system we'd calculate exact remaining time. For simplicity, we assume 1 hour format.
            return Result<bool>.Failure(
                new ErrorResult(
                    "RateLimitExceeded",
                    "Limits for recovery password exceeded, try again on 1 hour"
                )
            );
        }

        // Incremento los intentos
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
        };
        await _cache.SetStringAsync(ipRateLimitKey, (ipAttempts + 1).ToString(), options, ct);
        await _cache.SetStringAsync(emailRateLimitKey, (emailAttempts + 1).ToString(), options, ct);

        // Busco el usuario
        AstraVenturaAuth.Core.Domain.ValueObjects.Email emailVo;
        try
        {
            emailVo = new AstraVenturaAuth.Core.Domain.ValueObjects.Email(email);
        }
        catch (ArgumentException)
        {
            // Formato inválido, devuelvo éxito
            return Result<bool>.Success(true);
        }

        var user = await _userRepository.FindByEmailAsync(emailVo, ct);
        if (user == null)
        {
            // Siempre se retorna éxito incluso si el usuario no existe, para prevenir la enumeración de correos
            return Result<bool>.Success(true);
        }

        // Generamos el token
        var token = Guid.NewGuid().ToString("N");
        var tokenKey = $"pwd-reset:{token}";

        // Guardamos el token con 15 minutos de expiración
        var tokenOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
        };
        await _cache.SetStringAsync(tokenKey, user.Id.ToString(), tokenOptions, ct);

        // Enlace de reseteo de contraseña (url base desde variables de entorno/configuración)
        var frontendUrl = _configuration["FrontendBaseUrl"] ?? "https://astraventurauniversal.com";
        var resetLink = $"{frontendUrl.TrimEnd('/')}/reset-password?token={token}";

        // Envío el correo
        await _emailSender.SendPasswordResetEmailAsync(email, resetLink, ct);

        return Result<bool>.Success(true);
    }
}
