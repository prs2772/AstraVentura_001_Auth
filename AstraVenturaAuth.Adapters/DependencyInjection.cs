// Adapters/DependencyInjection.cs
using AstraVenturaAuth.Adapters.Drivens.Database;
using AstraVenturaAuth.Adapters.Drivens.Security;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Ports.Drivers;
using AstraVenturaAuth.Core.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstraVenturaAuth.Adapters;

public static class DependencyInjection
{
    public static IServiceCollection AddAdapters(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // DB, desde el puente de Factory o ya en docker desde Program.cs
        services.AddDbContext<AuthDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("AuthDb"))
        );

        // Inyecto mis implementaciones de los puertos (Drivens que se firman en Core, aqui los implemento)
        services.AddScoped<IUserRepository, UserRepository>(); // Uno por request
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>(); // No requiero más de una instancia, es stateless

        // Inyecto mis casos de uso que se registran y definen también dentro de Core
        services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<IRequestPasswordRecoveryUseCase, RequestPasswordRecoveryUseCase>();
        services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();

        // Configuración JWTm inyectando en mi clase JwtOption las opciones de configuración de appsettings.json
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));

        // Agregando Redis
        var redisConnection = config.GetConnectionString("Redis");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "AstraVentura_"; // Prefijo para que no se mezclen claves de Redis entre Apps
        });

        // Configuración de Proveedor de Correos
        services.AddSingleton<
            IEmailSender,
            AstraVenturaAuth.Adapters.Drivens.Notifications.SendGridEmailSender
        >();

        return services;
    }
}
