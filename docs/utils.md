# Comandos útiles

## La BD no tiene datos importantes y se quiere resetear

### Opcion A, manualmente indicar migraciones a remover

dotnet ef database drop --project AstraVenturaAuth.Adapters --startup-project AstraVenturaAuth.Api
dotnet ef migrations remove --project AstraVenturaAuth.Adapters --startup-project AstraVenturaAuth.Api
dotnet ef migrations add InitialCreate --project AstraVenturaAuth.Adapters --startup-project AstraVenturaAuth.Api
dotnet ef database update --project AstraVenturaAuth.Adapters --startup-project AstraVenturaAuth.Api

### Opcion B, B de Borrar todo

- Borrar la carpeta migraciones
- docker compose down -v
- docker compose up -d
