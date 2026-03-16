# Deploy en Azure

## 1. Crear el Registro de Imágenes (ACR)

Buscar **Container Registries** y crear uno nuevo, cuando se cree obtener en Access Keys el Login server, Username y Password

```bash
# 1. Inicia sesión el registro
docker login <login-server> -u <username> -p <password>

# 2. Construye la imagen etiquetada para Azure
docker build -t <login-server>/astraventura-auth:v1.0.0 .

# 3. Sube la imagen a la nube
docker push <login-server>/astraventura-auth:v1.0.0
```

### Ejemplo de implementación en auth

```bash
# 1. Inicia sesión el registro
docker login astraventuraauth-e2d6h4ajd7caejad.azurecr.io -u AstraVenturaAuth -p <password>

# 2. Construye la imagen etiquetada para Azure
docker build -t astraventuraauth-e2d6h4ajd7caejad.azurecr.io/astraventura-auth:v1.0.0 .

# 3. Sube la imagen a la nube
docker push astraventuraauth-e2d6h4ajd7caejad.azurecr.io/astraventura-auth:v1.0.0
```

## Crear la Base de Datos (Azure SQL)

Buscar SQL Databases
