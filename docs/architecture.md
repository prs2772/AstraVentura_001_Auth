# Hex/Ports and Adapter

## Diagrama general

         [ API Controller ]
                ↓
          (Driver Adapter)
                ↓
          IAuthenticateUser
                ↓
     [ Application Use Case ]
                ↓
IUserRepository | ITokenGenerator | IOAuthProvider
                ↓
      (Driven Adapters / Infra)

## Arquitectura del proyecto

AuthAstraVentura.sln
│
├── AuthAstraVentura.Core (El Hexágono - Sin dependencias externas)
│   ├── Domain
│   │   └── User.cs
│   │
│   ├── Ports
│   │   ├── Drivers (Primary / Inbound)
│   │   │   ├── IAuthenticateUserUseCase.cs
│   │   │   ├── IRegisterUserUseCase.cs
│   │   │   └── IRefreshTokenUseCase.cs
│   │   │
│   │   └── Drivens (Secondary / Outbound)
│   │       ├── IUserRepository.cs    <-- Para ir a la BD
│   │       ├── ITokenGenerator.cs <-- Para generar JWT/Refresh
│   │       └── IOAuthIdentityProvider.cs   <-- Para hablar con Google o otros oAuth providers (Driven)
│   │
│   ├── UseCases (Implementación de la lógica del hexágono se puede llamar UseCases o ApplicationServices en DDD)
│   │   ├── AuthenticateUserUseCase.cs
│   │   ├── RegisterUserUseCase.cs
│   │   └── RefreshTokenUseCase.cs
|   |
│   └── Dtos
│       ├── CredentialsDto.cs
│       ├── RegisterNewDto.cs
│       └── AuthenticatedUserDto.cs
│
└── AuthAstraVentura.Adapters (El exterior)
    ├── Drivers
    │   └── AuthController.cs
    │
    └── Drivens
        ├── Database (Implementa IForRepoQuerying)
        ├── Security (Implementa IForTokenOperations)
        └── GoogleAdapter (Implementa IForOAuthProvider)
