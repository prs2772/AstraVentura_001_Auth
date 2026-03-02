# Explicación del código

## Api

### Program.cs

En Program.cs se configura la aplicación, se registran los servicios y se configura la autenticación JWT.

Los middlewares por los que pasa, van decidiendo sobre la petición Http si se continúa, se interrumpe o si se modifica algún valor. Cada uno de ellos es:

```csharp
public async Task Invoke(HttpContext context)
{
    // Antes de que pase a la siguiente capa
    await _next(context);
    // Después de que pase por todas las capas
}
```

- builder.Services registra los servicios y dependencias en el **Contenedor de Inyección de Dependencias**.
    - AddControllers: Registra los controladores de la aplicación, soportando MVC, ApiController, IActionResult y otros como MapControllers() llamado más abajo.
    - AddEndpointsApiExplorer: Permite a Swagger detectar los endpoints de la API.
    - AddSwaggerGen: Registra el generador de Swagger, que permite documentar la API y se usa principalmente en desarrollo.
    - AddAdapters: Registra los servicios y dependencias de Adapters + Core.
    - AddAuthorization: Registra la autorización.
    - AddAuthentication: Registra la autenticación JWT como método por default de autenticación.
        - TokenValidationParameters, define como se va a validar el token. Indicamos 
            - ValidateIssuer: Indica si se debe validar el emisor del token.
            - ValidateAudience: Indica si se debe validar la audiencia del token.
            - ValidateLifetime: Indica si se debe validar que el token no haya expirado.
            - ValidateIssuerSigningKey: Indica si se debe validar que el token haya sido firmado por el emisor.
    - Redis: 20260301 prs - Se agrega el driver de Redis para cacheo de tokens
- En la pipeline (cadena de middelwares) por la que pasa la request Http, se define el **orden** por el cual se ejecutan los middelwares. Por ejemplo, primero se debe agregar UseAuthorization() y luego UseAuthentication() para que la autorización funcione correctamente.
    - AddAuthorization: Permite que valide si el usuario está autenticado y autorizado para acceder a un recurso.
    - UseHttpsRedirection: Redirige todas las peticiones HTTP a HTTPS.
    - UseAuthentication: Busca en el Header a Authorization, extrae el token y valida la firma. Habilita el funcionamiento a [Authorize], sin token se devuelve 401 (unauthorized).
    - UseAuthorization: Una vez autenticado, va a verificar si un los campos que ya puse arriba son correctos, evaluando policys, roles, claims para validacioens de acceso a endpoints más específicas.
    - MapControllers: Registra los controladores de la aplicación, registra los endpoints.

### Controllers

Los controladores son la puerta de entrada de la aplicación, reciben las peticiones HTTP y las envían a los casos de uso. En este caso AuthController recibe las peticiones HTTP y las envía a los casos de uso de register, login y refresh tokens.

## Adapters

Los adaptadores son los encargados de implementar los puertos de entrada y salida de la aplicación. En este caso AstraVenturaAuth.Adapters implementa los puertos de entrada y salida de la aplicación.

## Core

Core es el nucleo de la aplicación, contiene la lógica de negocio y define los puertos que se implementarán en los adaptadores. Por lo tanto, no tiene dependencias externas. En esta arquitecrtura, es el núcleo del hexágono.
