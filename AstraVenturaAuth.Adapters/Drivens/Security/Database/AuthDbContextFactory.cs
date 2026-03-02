using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AstraVenturaAuth.Adapters.Drivens.Database;

/// <summary>
/// Puente entre API y Adapters, API tiene la config. Indica a EF Core donde buscar el appsettings.json, el cual está en API.
/// La interfaz IDesignTimeDbContextFactory es para que EF Core pueda crear el contexto antes de hacer otra cosa
/// Solo la usamos para desarrollo, porque en docker se va a tomar todo lode Program por DI y esto se va a ignorar
/// </summary>
public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    // Es usado por la terminal, por eso el string[] args
    public AuthDbContext CreateDbContext(string[] args)
    {
        // Busca appsettings.json desde el proyecto Api
        var config = new ConfigurationBuilder()
            // SetBasePath coloca la ruta base para buscar el archivo, aqui lo mando para Api
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),
                         "../AstraVenturaAuth.Api"))
            // AddJsonFile agrega el archivo de configuración, igual el de abajo para desarrollo (opcional)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        // UseSqlServer indica a EF Core que use SQL Server con la cadena de conexión del json
        optionsBuilder.UseSqlServer(config.GetConnectionString("AuthDb"));

        return new AuthDbContext(optionsBuilder.Options);
    }
}
