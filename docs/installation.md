# Instalación y Configuración

## Requisitos Previos

Asegúrese de tener instaladas las siguientes herramientas en su entorno de desarrollo:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18.13.0 o superior)
- [Angular CLI](https://angular.io/cli) v21
- [MySQL Server](https://www.mysql.com/) 8.4.7
- [Docker](https://www.docker.com/) (Opcional, para contenerización)

## Guía de Instalación

### 1. Base de Datos (MySQL 8.4.7)

**Paso 1:** Iniciar el servicio de MySQL.

**Paso 2:** Ejecutar el script de inicialización para crear la base de datos y tablas.

```bash
# Desde la línea de comandos
mysql -u root -p < ../database/script.sql
```

### 2. Backend (.NET 10)

**Paso 1:** Configurar información sensible.

**Importante:** Este proyecto no incluye credenciales en el repositorio. Debe crear un archivo `appsettings.Local.json` en `backend/StudentEnrollment.Api/`.

Cree el archivo basándose en el siguiente ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=student_enrollment_db;User=root;Password=SU_PASSWORD;"
  },
  "JwtSettings": {
    "Secret": "SU_CLAVE_SECRETA_MUY_SEGURA_MINIMO_32_CHARS"
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "su_email@gmail.com",
    "Password": "su_app_password",
    "SenderEmail": "no-reply@university.com"
  }
}
```

**Paso 2:** Restaurar y ejecutar la API.

```bash
cd backend
dotnet restore
dotnet run --project StudentEnrollment.Api
```
La API estará disponible en `https://localhost:5001`.

### 3. Frontend (Angular 21)

**Paso 1:** Instalar dependencias.

```bash
cd frontend
npm install
```

**Paso 2:** Verificar la configuración de entorno (`src/environments/environment.ts`) para asegurar que apunte a la URL correcta del backend.

**Paso 3:** Iniciar el servidor de desarrollo.

```bash
ng serve
```
La aplicación estará disponible en `http://localhost:4200`.
