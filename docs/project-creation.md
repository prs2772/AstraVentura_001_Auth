# 📘 Project Creation on .NET Core 8

Comandos para generar la estructura Hex Architecture desde cero.

## 1.1 Crear la Solución y Capas

Para ver la arquitectura que se está utilizando se puede acceder a su documentación
→ [Detailed architecture](./architecture.md)

### Solución vacía

dotnet new sln -n AstraVenturaAuth

### Capas (Proyectos)

dotnet new classlib -n AstraVenturaAuth.Core             # Hexágono
dotnet new classlib -n AstraVenturaAuth.Adapters         # Adaptadores externos
dotnet new webapi -n AstraVenturaAuth.Api                # Api

### Agregar proyectos a la solución (.sln)

dotnet sln add AstraVenturaAuth.Core/AstraVenturaAuth.Core.csproj
dotnet sln add AstraVenturaAuth.Adapters/AstraVenturaAuth.Adapters.csproj
dotnet sln add AstraVenturaAuth.Api/AstraVenturaAuth.Api.csproj

## 1.2 Referencias entre Capas (Dependencias)

### Adapters usa Core

dotnet add AstraVenturaAuth.Adapters/AstraVenturaAuth.Adapters.csproj reference AstraVenturaAuth.Core/AstraVenturaAuth.Core.csproj

### Api usa Adapters y Core, es lo más externo posible

dotnet add AstraVenturaAuth.Api reference AstraVenturaAuth.Core
dotnet add AstraVenturaAuth.Api reference AstraVenturaAuth.Adapters

## 1.3 Agregando paquetes a la capa externa de Adapters y Api

### BCryptPasswordHasher

dotnet add AstraVenturaAuth.Adapters package BCrypt.Net-Next

### JwtTokenGenerator

dotnet add AstraVenturaAuth.Adapters package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.13

### EF Core

dotnet add AstraVenturaAuth.Adapters package Microsoft.EntityFrameworkCore --version 8.0.13
dotnet add AstraVenturaAuth.Adapters package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13  # o Npgsql si usas Postgres
dotnet add AstraVenturaAuth.Adapters package Microsoft.EntityFrameworkCore.Tools --version 8.0.13

Necesario para que dotnet ef encuentre el punto de entrada:
dotnet add AstraVenturaAuth.Api package Microsoft.EntityFrameworkCore.Design --version 8.0.13

## 1.4 Agregando la migración

### Crear la migración

dotnet ef migrations add InitialCreate \
    --project AstraVenturaAuth.Adapters \
    --startup-project AstraVenturaAuth.Api

### Aplicar a la BD

dotnet ef database update \
    --project AstraVenturaAuth.Adapters \
    --startup-project AstraVenturaAuth.Api

## 1.5 Agregando paquetes a la capa externa de Adapters y Api

### Redis Driver

dotnet add AstraVenturaAuth.Adapters package Microsoft.Extensions.Caching.StackExchangeRedis
