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

### Agregar proyectos a la solución (.sln)

dotnet sln add AstraVenturaAuth.Core/AstraVenturaAuth.Core.csproj
dotnet sln add AstraVenturaAuth.Adapters/AstraVenturaAuth.Adapters.csproj

## 1.2 Referencias entre Capas (Dependencias)

### Adapters usa Core

dotnet add AstraVenturaAuth.Adapters/AstraVenturaAuth.Adapters.csproj reference AstraVenturaAuth.Core/AstraVenturaAuth.Core.csproj
