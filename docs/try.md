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

---

## 4. Cambio de Contraseña (Change Password)

**Endpoint:** `POST /api/auth/change-password`

**Propósito:** Cambiar la contraseña del usuario.

**Headers:**
```
{
  "Authorization": "Bearer mi-jwt-token"
}
```

**Body:**
```json
{
  "currentPassword": "PasswordSeguro123!",
  "newPassword": "PasswordSeguro123!"
}
```

**Respuesta Exitosa (200):**
```json
{
    "message": "Your password has been successfully changed and all other sessions logged out."
}
```

**Respuesta Error (401):**
```json
{
    "code": "Unauthorized access",
    "message": "Missing user identity claim."
}
```

## 5. Recuperación de Contraseña (Password Recovery)

**Endpoint:** `POST /api/auth/forgot-password`

**Propósito:** Iniciar el proceso de recuperación de contraseña.

**Body:**
```
{
  "email": "miemail@correo.com"
}
```

**Body:**
```json
{
  "email": "miemail@correo.com"
}
```

**Respuesta Exitosa (200):**
```json
{
    "message": "If the email exists, a reset link will be sent."
}
```

## 6. Restablecimiento de Contraseña (Password Reset)

**Endpoint:** `POST /api/auth/reset-password`

**Propósito:** Restablecer la contraseña del usuario.

**Body:**
```json
{
  "token": "mi-token-recibido-por-correo",
  "newPassword": "newpassword123!-cumple-con-politicas"
}
```

**Respuesta Exitosa (200):**
```json
{
    "message": "Password has been successfully reset."
}
```
