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

AstraVenturaAuth.sln
│
├── AstraVenturaAuth.Core (El Hexágono - Sin dependencias externas)
│   ├── Common
│   │   ├── Result.cs
│   │   └── Errors
│   │       ├── ErrorResult.cs
│   │       └── AuthErrors.cs
│   │
│   ├── Domain
│   │   ├── User.cs
│   │   └── ValueObjects
│   │       ├── Email.cs
│   │       ├── Password.cs
│   │       ├── PasswordHash.cs
│   │       ├── PersonName.cs
│   │       └── UserId.cs
│   │
│   ├── Ports
│   │   ├── Drivers (Primary / Inbound)
│   │   │   ├── IAuthenticateUserUseCase.cs
│   │   │   ├── IRegisterUserUseCase.cs
│   │   │   └── IRefreshTokenUseCase.cs
│   │   │
│   │   └── Drivens (Secondary / Outbound)
│   │       ├── IUserRepository.cs
│   │       ├── IPasswordHasher.cs
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
│       ├── RegisterNewUserDto.cs
│       ├── RefreshTokenDto.cs
│       └── AuthenticatedUserDto.cs
│
└── AstraVenturaAuth.Adapters (El exterior)
    ├── Drivers
    │   └── AuthController.cs
    │
    └── Drivens
        ├── Database (Implementa IForRepoQuerying)
        ├── Security (Implementa IForTokenOperations)
        └── GoogleAdapter (Implementa IForOAuthProvider)
