# Student Enrollment System

Sistema de gestión de inscripciones universitarias con arquitectura Full Stack.

## Tecnologías

- **Backend**: .NET 10 (C#)
- **Frontend**: Angular 21
- **Base de Datos**: MySQL 8.4.7

## Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 o superior)
- [Angular CLI](https://angular.io/cli) v21
- [MySQL](https://www.mysql.com/) 8.4.7
- [Docker](https://www.docker.com/) (opcional)

## Configuración de Información Sensible

**IMPORTANTE:** Antes de inicializar el proyecto, debes configurar la información sensible que NO está incluida en el repositorio por seguridad.

### Archivos que NO se suben al repositorio:

- `backend/StudentEnrollment.Api/appsettings.Local.json`
- `backend/StudentEnrollment.Api/Logs/`

### Información sensible que debes configurar:

#### 1. **Base de Datos (MySQL)**
- **Usuario y contraseña** de MySQL
- Ubicación: `appsettings.json` o `appsettings.Local.json`

#### 2. **JWT Secret Key**
- **Clave secreta** para firma de tokens JWT (mínimo 32 caracteres)
- Genera una clave segura usando:
  ```bash
  # PowerShell
  [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
  ```
- Ubicación: `JwtSettings.Secret` en appsettings

#### 3. **Configuración de Correo Electrónico (SMTP)**
- **Host**: Servidor SMTP (ej: smtp.gmail.com)
- **Username**: Tu correo electrónico
- **Password**: Contraseña de aplicación (NO tu contraseña de Gmail)
  - Para Gmail: Genera una [contraseña de aplicación](https://support.google.com/accounts/answer/185833)
- **SenderEmail**: Correo que aparecerá como remitente
- Ubicación: `SmtpSettings` en appsettings

### Pasos para configurar:

1. **Copia el archivo de ejemplo:**
   ```bash
   cd backend/StudentEnrollment.Api
   copy appsettings.example.json appsettings.Local.json
   ```

2. **Edita `appsettings.Local.json`** con tus credenciales reales:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=student_enrollment_db;User=root;Password=TU_PASSWORD_MYSQL;"
     },
     "JwtSettings": {
       "Secret": "TU_CLAVE_JWT_SEGURA_MINIMO_32_CARACTERES"
     },
     "SmtpSettings": {
       "Host": "smtp.gmail.com",
       "Port": "587",
       "Username": "tu-correo@gmail.com",
       "Password": "tu_password_de_aplicacion",
       "SenderEmail": "tu-correo@gmail.com",
       "SenderName": "Student Enrollment System"
     }
   }
   ```

## Instalación y Configuración

### 1. Base de Datos (MySQL 8.4.7)

**Paso 1:** Iniciar el servidor MySQL

Asegúrate de que MySQL 8.4.7 esté instalado y el servicio esté corriendo.

**Paso 2:** Ejecutar el script de inicialización

El script crea automáticamente la base de datos `student_enrollment_db` y todas las tablas necesarias.

```bash
mysql -u root -p < database/script.sql
```

O desde el cliente MySQL:
```bash
mysql -u root -p
```

```sql
SOURCE database/script.sql;
```

### 2. Backend (.NET 10)

**Paso 1:** Navegar al directorio del backend
```bash
cd backend
```

**Paso 2:** Configurar información sensible

Crea el archivo `StudentEnrollment.Api/appsettings.Local.json` (ver sección [Configuración de Información Sensible](#configuración-de-información-sensible)) con tus credenciales:

- Contraseña de MySQL
- JWT Secret Key
- Credenciales SMTP

**Paso 3:** Restaurar dependencias y ejecutar

```bash
dotnet restore
dotnet build
dotnet run --project StudentEnrollment.Api
```

La API estará disponible en `https://localhost:5001` o `http://localhost:5000`

### 3. Frontend (Angular 21)

**Paso 1:** Navegar al directorio del frontend
```bash
cd frontend
```

**Paso 2:** Instalar dependencias
```bash
npm install
```

**Paso 3:** Configurar la URL del API (si es necesario)

Edita `src/environments/environment.ts` para apuntar a tu backend.

**Paso 4:** Ejecutar la aplicación
```bash
ng serve
```

La aplicación estará disponible en `http://localhost:4200`

## Orden de Inicio

Para inicializar la aplicación completa:

1. **Iniciar MySQL** (debe estar corriendo en el puerto 3306)
2. **Iniciar Backend** (ejecutar desde `/backend`)
3. **Iniciar Frontend** (ejecutar desde `/frontend`)

## Estructura del Proyecto

```
├── backend/                    # API .NET 10
│   ├── StudentEnrollment.Api/         # Capa de presentación
│   ├── StudentEnrollment.Application/ # Lógica de aplicación
│   ├── StudentEnrollment.Domain/      # Entidades y lógica de negocio
│   └── StudentEnrollment.Infrastructure/ # Acceso a datos
├── frontend/                   # Aplicación Angular 21
│   └── src/
└── database/                   # Scripts de base de datos
```

## Arquitectura

El backend sigue los principios de **Clean Architecture** y **CQRS**:

- **API**: Controladores y endpoints REST
- **Application**: Comandos, consultas y validadores (MediatR)
- **Domain**: Entidades, value objects y lógica de negocio
- **Infrastructure**: Repositorios, Entity Framework Core, servicios externos

## Licencia

Este proyecto es privado y confidencial.
