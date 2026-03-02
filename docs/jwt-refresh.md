# Estrategia de Refresh Tokens

Para poder interactuar dentro del ecosistema de AstraVentura, se requiere un par de tokens: Access Token y Refresh Token.

## Access Token

El Access Token es un token de corta duración que se utiliza para autenticar las peticiones a los servicios de AstraVentura. Dura no más de 15 minutos desde su emisión en la clase de JwtTokenGenerator.

## Refresh Token

El Refresh Token es un token de larga duración que se utiliza para obtener nuevos Access Tokens sin necesidad de volver a iniciar sesión. Dura no más de 7 días desde su emisión en la clase de JwtTokenGenerator.

## Flujo de Refresh Tokens

1. El usuario inicia sesión y obtiene un par de tokens.
2. El usuario utiliza el Access Token para autenticar sus peticiones, por ejemplo iniciar sesión.
3. Cuando el Access Token expira después de 15 min por ejemplo, al usarlo se convierte en 401 unauthorized. Por lo tanto el frotnend debe detectarlo y enviar el refresh token a /refresh para solicitar un nuevo par sin iniciar sesión.
4. ValidateRefreshTokenAsync determina si el token de refresh no ha caducado, si existe y si es válido.
5. El Refresh Token se invalida después de ser utilizado, se generará un nuevo par de tokens.
6. Para un logout, se debe invalidar el refresh token  (InvalidateRefreshTokenAsync) o cuando se use. Esto porque el JWT no se puede borrar, vive hasta que expire.
