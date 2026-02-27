# Hex/Ports and Adapter

## Arquitectura del proyecto

AuthAstraVentura.sln
│
├── AuthAstraVentura.Core (El Hexágono - Sin dependencias externas)
│   ├── Domain
│   │   └── User.cs
│   │
│   ├── Ports
│   │   ├── Drivers (Primary / Inbound)
│   │   │   └── IForAuthenticating.cs  <-- Contrato de entrada
│   │   │
│   │   └── Drivens (Secondary / Outbound)
│   │       ├── IForRepoQuerying.cs    <-- Para ir a la BD
│   │       ├── IForTokenOperations.cs <-- Para generar JWT/Refresh
│   │       └── IForOAuthProvider.cs   <-- Para hablar con Google o otros oAuth providers (Driven)
│   │
│   ├── Dtos
│   │   ├── CredentialsDto.cs
│   │   ├── RegisterNewDto.cs
│   │   └── AuthenticatedUserDto.cs
│   │
│   └── Application (Implementación de la lógica del hexágono)
│       └── AuthAstraApp.cs            <-- Implementa IForAuthenticating
│
├── AuthAstraVentura.Adapters (El exterior)
│   ├── Drivers
│   │   └── Api (Controllers que llaman a AuthAstraApp)
│   │
│   └── Drivens
│       ├── Database (Implementa IForRepoQuerying)
│       ├── Security (Implementa IForTokenOperations)
│       └── GoogleAdapter (Implementa IForOAuthProvider)
