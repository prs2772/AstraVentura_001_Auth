# Astra Ventura Auth

Repositorio GitHub: AstraVentura_001_Auth

## 📌 Próposito

Decir si alguien está autenticado y decidir quién es y para que roles tiene acceso en el ecosistema de Astra Ventura Universal.

## 🎯 Alcance

El ecosistema de Astra Ventura Universal, se comunica con otros puertos y adaptadores así como otras arquitecturas dentro del mismo para proporcionar una autenticación y roles que se generarán en un Jwt y que a su vez servirá a las otras entidades de Astra Ventura para validar los tokens emitidos.

Se puede ver una explicación general del código en el archivo docs/explanation.md
→ [Explicación general](docs/explanation.md)

## 🏗 Arquitectura

Se usa la arquitectura de puertos y adaptadores o conocida como hexagonal.
→ [Arquitectura detallada](docs/architecture.md)

Para el manejo de JWT y Refresh Tokens se usa una estrategia de par de tokens, el access token es de corta duración y el refresh token es de larga duración, el refresh token se almacena en Redis.
→ [JWT Refresh](docs/jwt-refresh.md)

Para revisar cómo se fué creando el proyecto, revisar el archivo project-creation.md
→ [Project Creation](docs/project-creation.md)

## 🚀 Ejecución

Ejecutar docker-compose up -d para primera creación de los servicios, se cargará todo lo necesario para el funcionamiento de la aplicación. Para detener los servicios ejecutar docker-compose down.

Para deployar en azure revisar el archivo azure.md
→ [Azure](docs/azure.md)

## 🧪 Try me

Revisar el archivo try.md para ver cómo se puede usar la API.
→ [Try me](docs/try.md)

## 🛠️ Utilerías

Revisar utils.md para ver comandos útiles para el manejo de la aplicación.
→ [Utilerías](docs/utils.md)

## 📖 Documentation

- [Arquitectura detallada](docs/architecture.md)
- [JWT Refresh](docs/jwt-refresh.md)
- [Project Creation](docs/project-creation.md)
- [Explicación general](docs/explanation.md)
- [Try me](docs/try.md)
- [Utilerías](docs/utils.md)
- [Azure](docs/azure.md)
