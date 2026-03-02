# 🚀 API Endpoints (Auth Service)

## 1. Registro (Sign Up)

**Endpoint:** `POST /api/auth/register`

**Propósito:** Crear un nuevo usuario.

**Body:**
```json
{
  "email": "miemail@correo.com",
  "password": "PasswordSeguro123!",
  "nombre": "Paris",
  "apellidoPrincipal": "Ramírez",
  "apellidoSecundario": "Mi Apellido Secundario"
}

```

**Respuesta Exitosa (201):**
```json
{
    "id": "user-id",
    "email": "miemail@correo.com",
    "name": "Paris Ramírez Mi Apellido Secundario",
    "accessToken": "mi-jwt-token",
    "refreshToken": "mi-refresh-token",
    "expiresAt": "fecha-expiracion"
}
```

---

## 2. Inicio de Sesión (Login)

**Endpoint:** `POST /api/auth/login`

**Propósito:** Autenticarse y obtener el par de Tokens.

**Body:**
```json
{
  "email": "miemail@correo.com",
  "password": "PasswordSeguro123!"
}
```

**Respuesta Exitosa (200):**
```json
{
    "id": "user-id",
    "email": "miemail@correo.com",
    "name": "Paris Ramírez Mi Apellido Secundario",
    "accessToken": "mi-jwt-token",
    "refreshToken": "mi-refresh-token",
    "expiresAt": "fecha-expiracion"
}
```

---

## 3. Validación de Token (Health Check)

**Endpoint:** `POST /api/auth/refresh`

**Propósito:** Generar un nuevo token

**Headers:**
```
{
  "refreshToken": "mi-refresh-token"
}
```

**Respuesta Exitosa (200):**
```json
{
    "id": "user-id",
    "email": "miemail@correo.com",
    "name": "Paris Ramírez Mi Apellido Secundario",
    "accessToken": "mi-jwt-token",
    "refreshToken": "mi-refresh-token",
    "expiresAt": "fecha-expiracion"
}
```
