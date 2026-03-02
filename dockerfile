FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AstraVenturaAuth.Api/AstraVenturaAuth.Api.csproj", "AstraVenturaAuth.Api/"]
COPY ["AstraVenturaAuth.Core/AstraVenturaAuth.Core.csproj", "AstraVenturaAuth.Core/"]
COPY ["AstraVenturaAuth.Adapters/AstraVenturaAuth.Adapters.csproj", "AstraVenturaAuth.Adapters/"]

RUN dotnet restore "AstraVenturaAuth.Api/AstraVenturaAuth.Api.csproj"

COPY . .
WORKDIR "/src/AstraVenturaAuth.Api"
RUN dotnet build "AstraVenturaAuth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AstraVenturaAuth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exponer el puerto que usa ASP.NET Core por defecto en .NET 8
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AstraVenturaAuth.Api.dll"]
